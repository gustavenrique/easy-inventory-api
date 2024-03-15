using Stoquei.Domain.Enums;
using System.Net;
using System.Net.Mail;

namespace Stoquei.Domain.Dtos
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool Ativo { get; set; }
        public bool Admin { get; set; }
        public bool AlterarSenha { get; set; }
        public DateTime? UltimaAlteracaoSenha { get; set; }
        public List<Acesso> Acessos { get; set; }
        public string SenhaCriptografada() => string.IsNullOrEmpty(Senha) ? string.Empty
            : BitConverter
                .ToString(new System.Security.Cryptography.SHA256Managed()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(Senha)))
                .Replace("-", string.Empty).ToLower();

        public UsuarioDto(int id, string usuario, string senha, string email, bool ativo, bool admin, List<Acesso> acessos, bool alterarSenha, DateTime ultimaAlteracaoSenha)
        {
            Id = id;
            Usuario = usuario;
            Email = email;
            Senha = senha;
            Ativo = ativo;
            Admin = admin;
            Acessos = acessos;
            AlterarSenha = alterarSenha;
            UltimaAlteracaoSenha = ultimaAlteracaoSenha;
        }

        public UsuarioDto(string usuario, string email, bool ativo, bool admin, List<Acesso> acessos)
        {
            Usuario = usuario;
            Senha = GerarSenhaAleatoria();
            Email = email;
            Ativo = ativo;
            Admin = admin;
            Acessos = acessos;
        }

        public UsuarioDto(int id, string usuario, string email, bool ativo, bool admin, List<Acesso> acessos)
        {
            Id = id;
            Usuario = usuario;
            Email = email;
            Ativo = ativo;
            Admin = admin;
            Acessos = acessos;
        }

        public UsuarioDto(int id, string usuario)
        {
            Id = id;
            Usuario = usuario;
        }
        
        public UsuarioDto() { }

        private string GerarSenhaAleatoria()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        public void EnviarEmailComSenha(string fromMail, string fromPass)
        {
            var msg = new MailMessage();

            msg.From = new MailAddress(fromMail);
            msg.Subject = "Stoquei - Seu login para acesso";
            msg.To.Add(new MailAddress(this.Email));
            msg.Body = $@"<!DOCTYPE html><html xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office' lang='en'><head><title></title><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta name='viewport' content='width=device-width,initial-scale=1'><!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]--><style>
            *{{box-sizing:border-box}}body{{margin:0;padding:0}}a[x-apple-data-detectors]{{color:inherit!important;text-decoration:inherit!important}}#MessageViewBody a{{color:inherit;text-decoration:none}}p{{line-height:inherit}}.desktop_hide,.desktop_hide table{{mso-hide:all;display:none;max-height:0;overflow:hidden}}@media (max-width:720px){{.social_block.desktop_hide .social-table{{display:inline-block!important}}.image_block img.big,.row-content{{width:100%!important}}.mobile_hide{{display:none}}.stack .column{{width:100%;display:block}}.mobile_hide{{min-height:0;max-height:0;max-width:0;overflow:hidden;font-size:0}}.desktop_hide,.desktop_hide table{{display:table!important;max-height:none!important}}}}
            </style></head><body style='background-color:#fff;margin:0;padding:0;-webkit-text-size-adjust:none;text-size-adjust:none'><table class='nl-container' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0;background-color:#fff'><tbody><tr><td><table class='row row-1' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0'><tbody><tr><td><table 
            class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0;color:#000;width:700px' width='700'><tbody><tr><td class='column column-1' width='100%' style='mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top;padding-top:30px;padding-bottom:20px;border-top:0;border-right:0;border-bottom:0;border-left:0'><table class='image_block block-1' width='100%' border='0' 
            cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0'><tr><td class='pad' style='width:100%;padding-right:0;padding-left:0'><div class='alignment' align='center' style='line-height:10px'><img class='big' src='https://d15k2d11r6t6rl.cloudfront.net/public/users/Integrators/0db9f180-d222-4b2b-9371-cf9393bf4764/0bd8b69e-4024-4f26-9010-6e2a146401fb/stoquei%20logo.png' style='display:block;height:auto;border:0;width:546px;max-width:100%' width='546'></div>
            </td></tr></table></td></tr></tbody></table></td></tr></tbody></table><table class='row row-2' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0'><tbody><tr><td><table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0;background-color:#f6f6f6;color:#000;width:700px' width='700'><tbody><tr><td 
            class='column column-1' width='100%' style='mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top;padding-top:40px;padding-bottom:0;border-top:0;border-right:0;border-bottom:0;border-left:0'><table class='text_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0;word-break:break-word'><tr><td class='pad' 
            style='padding-bottom:10px;padding-left:20px;padding-right:20px;padding-top:10px'><div style='font-family:sans-serif'><div class style='font-size:12px;mso-line-height-alt:14.399999999999999px;color:#555;line-height:1.2;font-family:Arial,Helvetica Neue,Helvetica,sans-serif'><p style='margin:0;font-size:14px;text-align:center;mso-line-height-alt:16.8px'><strong><span style='font-size:24px;'>Bem-vindo à plataforma!</span></strong></p></div></div></td></tr></table><table class='text_block block-2' 
            width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0;word-break:break-word'><tr><td class='pad' style='padding-bottom:10px;padding-left:30px;padding-right:30px;padding-top:10px'><div style='font-family:sans-serif'><div class style='font-size:12px;mso-line-height-alt:18px;color:#555;line-height:1.5;font-family:Arial,Helvetica Neue,Helvetica,sans-serif'><p style='margin:0;font-size:14px;mso-line-height-alt:21px'>
            <span style='font-size:14px;'><strong>Este será o seu login:</strong></span></p><p style='margin:0;font-size:14px;mso-line-height-alt:18px'>&nbsp;</p><p style='margin:0;font-size:14px;mso-line-height-alt:21px'><span style='font-size:14px;'><strong>Usuario: {this.Usuario}</strong></span></p><p style='margin:0;font-size:14px;mso-line-height-alt:21px'><span style='font-size:14px;'><strong>Senha: {this.Senha}</strong></span></p><p style='margin:0;font-size:14px;mso-line-height-alt:18px'>&nbsp;</p><p 
            style='margin:0;font-size:14px;mso-line-height-alt:21px'><span style='font-size:14px;'><strong>Tal senha é temporária, então, assim que fizer o primeiro login, terá de alterá-la.</strong></span></p><p style='margin:0;font-size:14px;mso-line-height-alt:21px'><span style='font-size:14px;'><strong>A imagem abaixo é a tela de login. Clique no botão 'Acessar plataforma' para usá-la.</strong></span></p><p style='margin:0;font-size:14px;mso-line-height-alt:18px'>&nbsp;</p></div></div></td></tr></table>
            <table class='image_block block-3' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0'><tr><td class='pad' style='width:100%;padding-right:0;padding-left:0'><div class='alignment' align='center' style='line-height:10px'><img class='big' src='https://d15k2d11r6t6rl.cloudfront.net/public/users/Integrators/0db9f180-d222-4b2b-9371-cf9393bf4764/0bd8b69e-4024-4f26-9010-6e2a146401fb/print%20tela%20login.png' 
            style='display:block;height:auto;border:0;width:700px;max-width:100%' width='700'></div></td></tr></table></td></tr></tbody></table></td></tr></tbody></table><table class='row row-3' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0'><tbody><tr><td><table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' 
            style='mso-table-lspace:0;mso-table-rspace:0;color:#000;width:700px' width='700'><tbody><tr><td class='column column-1' width='100%' style='mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top;padding-top:5px;padding-bottom:40px;border-top:0;border-right:0;border-bottom:0;border-left:0'><table class='button_block block-1' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0'><tr><td 
            class='pad'><div class='alignment' align='center'><!--[if mso]><v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' href='' style='height:48px;width:228px;v-text-anchor:middle;' arcsize='105%' stroke='false' fillcolor='#3AAEE0'><w:anchorlock/><v:textbox inset='0px,0px,0px,0px'><center style='color:#ffffff; font-family:Arial, sans-serif; font-size:16px'><![endif]-->
            <a href='http://127.0.0.1:5173' target='_blank' style='text-decoration:none;display:inline-block;color:#ffffff;background-color:#3AAEE0;border-radius:50px;width:auto;border-top:0px solid transparent;font-weight:undefined;border-right:0px solid transparent;border-bottom:0px solid transparent;border-left:0px solid transparent;padding-top:8px;padding-bottom:8px;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:16px;text-align:center;mso-border-alt:none;word-break:keep-all;'><span style='padding-left:40px;padding-right:40px;font-size:16px;display:inline-block;letter-spacing:normal;'><span style='word-break: break-word; line-height: 32px;' dir='ltr'><strong>Acessar plataforma</strong></span></span></a>
            <!--[if mso]></center></v:textbox></v:roundrect><![endif]--></div></td></tr></table></td></tr></tbody></table></td></tr></tbody></table><table class='row row-4' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0'><tbody><tr><td><table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0;color:#000;width:700px' 
            width='700'><tbody><tr><td class='column column-1' width='100%' style='mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top;padding-top:25px;padding-bottom:25px;border-top:0;border-right:0;border-bottom:0;border-left:0'><table class='social_block block-1' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0'><tr><td class='pad'><div class='alignment' align='center'><table 
            class='social-table' width='52px' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0;display:inline-block'><tr><td style='padding:0 10px 0 10px'><a href='https://www.linkedin.com/in/gustavenrique' target='_blank'><img src='https://app-rsrc.getbee.io/public/resources/social-networks-icon-sets/circle-color/linkedin@2x.png' width='32' height='32' alt='LinkedIn' title='LinkedIn' style='display:block;height:auto;border:0'></a></td>
            </tr></table></div></td></tr></table><table class='text_block block-2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace:0;mso-table-rspace:0;word-break:break-word'><tr><td class='pad'><div style='font-family:sans-serif'><div class style='font-size:12px;mso-line-height-alt:14.399999999999999px;color:#555;line-height:1.2;font-family:Arial,Helvetica Neue,Helvetica,sans-serif'><p 
            style='margin:0;font-size:14px;text-align:center;mso-line-height-alt:16.8px'><span style='font-size:12px;'><strong>Our mailing address:</strong></span></p><p style='margin:0;font-size:14px;text-align:center;mso-line-height-alt:16.8px'><span style='font-size:12px;'>atendimento@stoquei.com</span></p></div></div></td></tr></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table><!-- End --><div style='background-color:transparent;'>
            <div style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;' class='block-grid '>
            <div style='border-collapse: collapse;display: table;width: 100%;background-color:transparent;'>
            <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='background-color:transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width: 500px;'><tr class='layout-full-width' style='background-color:transparent;'><![endif]-->
            <!--[if (mso)|(IE)]><td align='center' width='500' style=' width:500px; padding-right: 0px; padding-left: 0px; padding-top:15px; padding-bottom:15px; border-top: 0px solid transparent; border-left: 0px solid transparent; border-bottom: 0px solid transparent; border-right: 0px solid transparent;' valign='top'><![endif]-->
            <div class='col num12' style='min-width: 320px;max-width: 500px;display: table-cell;vertical-align: top;'>
            <div style='background-color: transparent; width: 100% !important;'>
            <!--[if (!mso)&(!IE)]><!--><div style='border-top: 0px solid transparent; border-left: 0px solid transparent; border-bottom: 0px solid transparent; border-right: 0px solid transparent; padding-top:15px; padding-bottom:15px; padding-right: 0px; padding-left: 0px;'>
            <!--<![endif]-->
            <!--[if (!mso)&(!IE)]><!-->
            </div><!--<![endif]-->
            </div>
            </div>
            <!--[if (mso)|(IE)]></td></tr></table></td></tr></table><![endif]-->
            </div>
            </div>
            </div></body></html>";
            msg.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPass),
                EnableSsl = true
            };

            smtpClient.Send(msg);
        }
    }
}
