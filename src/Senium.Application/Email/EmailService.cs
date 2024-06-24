using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Senium.Application.Notifications;
using Senium.Core.Settings;
using Senium.Domain.Entities;

namespace Senium.Application.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly AppSettings _appSettings;
    
    public EmailService(IOptions<EmailSettings> emailSettings, IOptions<AppSettings> appSettings)
    {
        _emailSettings = emailSettings.Value;
        _appSettings = appSettings.Value;
    }
    
    public async Task EnviarEmailRecuperarSenhaUsuario(Usuario usuario)
    {
        var url = $"{_appSettings.UrlComum}/resetar-senha?token={usuario.TokenDeResetSenha}";
        var body = 
            $"Olá {usuario.Nome},<br><br>" +
            "Você solicitou a redefinição de senha para a sua conta na Senium Talentos. Para continuar, clique no botão abaixo e siga as instruções para criar uma nova senha segura:<br><br>" +
            $"<a href='{url}'>Redefinir Senha</a><br><br>" +
            "Se você não solicitou essa alteração, por favor, ignore este e-mail ou entre em contato conosco imediatamente.<br><br>" +
            "Atenciosamente, Equipe Senium Talentos";

        var mailData = new MailData
        {
            EmailSubject = "Redefina sua senha agora mesmo!",
            EmailBody = body,
            EmailToId = usuario.Email
        };


        await SendEmailAsync(mailData);
    }
    
    public async Task EnviarEmailRecuperarSenhaAdministrador(Administrador administrador)
    {
        var url = $"{_appSettings.UrlGestao}/resetar-senha?token={administrador.TokenDeResetSenha}";
        var body = 
            $"Olá {administrador.Nome},<br><br>" +
            "Você solicitou a redefinição de senha para a sua conta na Senium Talentos. Para continuar, clique no botão abaixo e siga as instruções para criar uma nova senha segura:<br><br>" +
            $"<a href='{url}'>Redefinir Senha</a><br><br>" +
            "Se você não solicitou essa alteração, por favor, ignore este e-mail ou entre em contato conosco imediatamente.<br><br>" +
            "Atenciosamente, Equipe Senium Talentos";

        var mailData = new MailData
        {
            EmailSubject = "Redefina sua senha agora mesmo!",
            EmailBody = body,
            EmailToId = administrador.Email
        };


        await SendEmailAsync(mailData);
    }
    
    public async Task SendEmailAsync(MailData mailData)
    {
        var toEmail = mailData.EmailToId;
        var user = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(_emailSettings.User));
        var password = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(_emailSettings.Password));

        var smtpClient = new SmtpClient(_emailSettings.Server)
        {
            Port = _emailSettings.Port,
            Credentials = new NetworkCredential(user, password),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage(user, toEmail)
        {
            Subject = mailData.EmailSubject,
            Body = mailData.EmailBody,
            IsBodyHtml = true
        };

        try
        {
            await Task.Run(() => smtpClient.Send(mailMessage));
        }
        catch (Exception)
        {
            var notificator = new Notificator();
            notificator.Handle("Ocorreu um erro ao enviar o e-mail");
        }
    }
    
}