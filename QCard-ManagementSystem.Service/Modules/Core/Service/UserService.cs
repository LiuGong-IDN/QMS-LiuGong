using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExcelDataReader;
using Hangfire.Common;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.General;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service;
using QCard.ManagementSystem.Service.Modules.Core.Validator;
using static System.Net.WebRequestMethods;
using SymmetricSecurityKey = Microsoft.IdentityModel.Tokens.SymmetricSecurityKey;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class UserService : DbService
    {
        private readonly IEmailService _emailService;
        private readonly TimeSpan TokenTtl = TimeSpan.FromMinutes(30);
        private readonly IWebHostEnvironment _env;
        public UserService(IDbHelper db, IEmailService emailService, IWebHostEnvironment env) : base(db)
        {
            _emailService = emailService;
            _env = env;
        }

        public DataSourceResult GetDataSourceResult(DataSourceRequest request)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();
            try
            {
                result = Db.UserRepository.GetDataSourceResult(request);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicResponse Save(UserRequest user)
        {
            string methodName = "Save";
            BasicResponse message = new();
            string passwordHash = string.Empty;
            try
            {
                //validator
                UserValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(user);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    throw new Exception(JsonSerializer.Serialize(validatorResult.Errors));
                }
                if (user.PASSWORD != null)
                {
                    passwordHash = PasswordHashing.Hash(user.PASSWORD);
                }

                var result = Db.UserRepository.Save(user, passwordHash, SessionManager.Current);

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Update Data");
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                message.Success = false;
                message.Message = ex.Message;
            }

            return message;
        }
        public async Task<BasicResponse> Register(UserRequest user)
        {
            string methodName = "Save";
            BasicResponse message = new();
            var by = "REGISTRATION";
            try
            {
                //validator
                UserValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(user);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    throw new Exception(JsonSerializer.Serialize(validatorResult.Errors));
                }

                var passwordHash = PasswordHashing.Hash(user.PASSWORD);
                var result = Db.UserRepository.Save(user, passwordHash, by);

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;

                    //send email
                    var fullName = string.IsNullOrWhiteSpace(user.LAST_NAME)
                                    ? user.FIRST_NAME
                                    : $"{user.FIRST_NAME} {user.LAST_NAME}";
                    var dateNow = DateTime.Now;
                    string formattedDate = dateNow.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("en-US"));
                    var formattedCardNumber = FormatCardNumber(user.QCARD);
                    var formattedCardNumberCard = FormatCardNumberCard(user.QCARD);
                    var validThru = dateNow.AddYears(1).ToString("MM/yy");
                    string subject = "Q-CARD REGISTRATION";
                    string body = $@"
                        <html>
                            <body style='font-family: -apple-system, BlinkMacSystemFont, `Segoe UI`, Roboto, Oxygen, Ubuntu, Cantarell, `Helvetica Neue`, sans-serif; background:#f5f5f5; padding:20px;'>
                            <div style='max-width:600px; margin:0 auto; background:#fff; padding:20px; border-radius:10px; box-shadow:0 4px 12px rgba(0,0,0,0.1);'>

                                <h2 style='color:#4CAF50;'>Registration Successful</h2>
                                <p>Hello <b>{fullName}</b>,</p>
                                <p>Congratulations! Your account registration on <b>Q-Card - Management System</b> was successful.</p>
                                <p>Here are your account details:</p>
                                <ul>
                                    <li>Email: {user.EMAIL}</li>
                                    <li>Phone Number: {user.PHONE_NUMBER}</li>
                                    <li><b>Q-Card No.: {formattedCardNumber}</b></li>
                                    <li>Registration Date: {formattedDate}</li>
                                </ul>

                                <p>Please log in using your registered email/phone number and password.</p>
                                <br/>
                                <p style='font-size:12px;color:#888;'>This message was sent automatically by the system. Please do not reply to this email.</p>
                            </div>
                            </body>
                        </html>"
                    ;

                    var logoPath = Path.Combine(_env.WebRootPath, "img", "liugong.png");
                    var logoBytes = System.IO.File.ReadAllBytes(logoPath);

                    await _emailService.SendEmailWithInlineImageAsync(
                        user.EMAIL,
                        subject, body,
                        inlineAttachments: new Dictionary<string, byte[]>
                        {
                            { "LogoImage", logoBytes }
                        }
                    );
                }
                else
                {
                    throw new Exception("Failed Update Data");
                }

                //new ActivityLogService(Db).CreateSuccessLog(methodName, "Registration");
            }
            catch (Exception ex)
            {
                //new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                message.Success = false;
                message.Message = ex.Message;
            }

            return message;
        }
        private string FormatCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return string.Empty;

            // Ambil hanya digit
            cardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());

            // Kelompokkan per 4 digit, dan sertakan sisa digit
            var splitCardNumber = Enumerable.Range(0, (cardNumber.Length + 3) / 4)
                                  .Select(i => cardNumber.Substring(i * 4, Math.Min(4, cardNumber.Length - i * 4)));
            var formattedCardNumber = string.Join("-", splitCardNumber);
            return formattedCardNumber;
        }
        private string FormatCardNumberCard(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return string.Empty;

            // Ambil hanya digit
            cardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());

            // Kelompokkan per 4 digit, dan sertakan sisa digit
            var splitCardNumber = Enumerable.Range(0, (cardNumber.Length + 3) / 4)
                                  .Select(i => cardNumber.Substring(i * 4, Math.Min(4, cardNumber.Length - i * 4)));
            var formattedCardNumber = string.Join(" ", splitCardNumber);
            return formattedCardNumber;
        }
        public BasicResponse Delete(string id)
        {
            string methodName = "Delete";
            BasicResponse message = new();
            try
            {
                var result = Db.UserRepository.Delete(id);

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Delete Data");
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                message.Success = false;
                message.Message = ex.Message;
            }

            return message;
        }
        public BasicResponse SendResetLink(string email, string resetBaseUrl)
        {
            var response = new BasicResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    response.Success = false;
                    response.Validate = false;
                    response.Message = "Email tidak boleh kosong.";
                    return response;
                }
                var empty = string.Empty;
                var user = Db.UserRepository.ListUser("GetList", empty, empty, email).FirstOrDefault();

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "No user found from the entered email";
                    return response;
                }
                if (string.IsNullOrWhiteSpace(resetBaseUrl))
                {
                    response.Success = false;
                    response.Validate = false;
                    response.Message = "The reset base URL is not configured.";
                    return response;
                }

                var token = GenerateToken();
                var tokenHash = HashToken(token);

                var resetToken = new PasswordResetTokenRequest
                {
                    Email = user.EMAIL,
                    TokenHash = tokenHash,
                    CreatedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.Add(TokenTtl),
                    IsUsed = false
                };

                Db.UserRepository.InsertPasswordResetToken(resetToken);

                var resetLink = $"{resetBaseUrl}?token={WebUtility.UrlEncode(token)}";

                // Prepare email body
                var fullName = string.IsNullOrWhiteSpace(user.LAST_NAME) ? user.FIRST_NAME : $"{user.FIRST_NAME} {user.LAST_NAME}";
                var body = $@"
                    <html>
                      <body style='font-family: -apple-system, BlinkMacSystemFont, `Segoe UI`, Roboto, Oxygen, Ubuntu, Cantarell, `Helvetica Neue`, sans-serif;'>
                        <p>Hello <b>{WebUtility.HtmlEncode(fullName)}</b>,</p>
                        <p>We received a request to reset your account password. Click the button below to change your password. This link will expire in {TokenTtl.TotalMinutes} minutes.</p>
                        <p><a href='{resetLink}' style='display:inline-block;padding:10px 16px;background:#2563EB;color:#fff;text-decoration:none;border-radius:6px;'>Reset Password</a></p>
                        <p>If you did not request a password reset, please ignore this email.</p>
                        <hr/>
                        <p style='font-size:12px;color:#666'>This message was sent automatically. Please do not reply to this email.</p>
                      </body>
                    </html>"
                ;


                try
                {
                    _emailService.SendEmailAsync(user.EMAIL, "Reset Password Q-Card - Management System", body).GetAwaiter().GetResult();
                }
                catch (Exception exEmail)
                {
                    response.Success = false;
                    response.Validate = false;
                    response.Message = "Failed to send email. Try again later. " + exEmail.StackTrace;
                    return response;
                }

                response.Success = true;
                response.Message = "Reset Password Link has been sent to the entered email.";
                return response;
            }
            catch (Exception ex)
            {
                // Catch-all
                response.Success = false;
                response.Validate = false;
                response.Message = "An error occurred when processing this request. " + ex.StackTrace;
                return response;
            }
        }

        public BasicResponse ResetPassword(ResetPasswordUpdateRequest request)
        {
            var response = new BasicResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(request.Token))
                {
                    response.Success = false;
                    response.Validate = false;
                    response.Message = "Token tidak boleh kosong.";
                    return response;
                }

                // Hash token and new password
                var tokenHash = HashToken(request.Token);
                var newPasswordHash = PasswordHashing.Hash(request.NewPassword);

                // Call stored procedure via repository that validates token and updates password atomically
                var result = Db.UserRepository.ValidateAndUsePasswordResetToken(tokenHash, newPasswordHash);

                if (result == 1)
                {
                    response.Success = true;
                    response.Message = "Your password has been successfully changed. Please log in using your new password.";
                    return response;
                }

                // result == 0 -> token invalid/expired/used
                response.Success = false;
                response.Validate = false;
                response.Message = "The reset password link is invalid or has expired.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Validate = false;
                response.Message = "An error occurred while changing the password. " + ex.StackTrace;
                // log ex
                return response;
            }
        }

        public ExcelResponse HandleExcelUpload(IEnumerable<IFormFile> upload)
        {
            const string methodName = "HandleExcelUpload";
            var users = new List<UserUpload>();
            ExcelResponse response;

            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var stream = new MemoryStream())
                {
                    foreach (var file in upload)
                    {
                        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                            throw new Exception("File format only support .xlsx");

                        file.CopyTo(stream);
                        stream.Position = 0;

                        using var reader = ExcelReaderFactory.CreateReader(stream);
                        int pos = 0;
                        int step = 10;

                        while (reader.Read())
                        {
                            string itemMessage = "";
                            pos++;

                            if (reader.Depth == 0 || pos <= step || reader.GetValue(0) == null)
                            {
                                if (pos == step)
                                {
                                    if (reader.FieldCount != 8)
                                        throw new Exception("Format Excel Invalid");

                                    var titleMerge = string.Concat(reader.GetValue(0)?.ToString(), "|", reader.GetValue(1)?.ToString(), "|", reader.GetValue(2)?.ToString(), "|", reader.GetValue(3)?.ToString(), "|", reader.GetValue(4)?.ToString(), "|", reader.GetValue(5)?.ToString(), "|", reader.GetValue(6)?.ToString(), "|", reader.GetValue(7)?.ToString());

                                    if (!titleMerge.Trim().Equals("Action|No|First Name|Last Name|Phone Number|Email|Password|Role Name"))
                                        throw new Exception("Format Excel Invalid");
                                }

                                continue;
                            }

                            if (!string.IsNullOrEmpty(reader.GetValue(0)?.ToString()))
                            {
                                var itemACTION = reader.GetValue(0)?.ToString() ?? string.Empty;
                                var itemFIRST_NAME = reader.GetValue(2)?.ToString() ?? string.Empty;
                                var itemLAST_NAME = reader.GetValue(3)?.ToString() ?? string.Empty;
                                var itemPHONE_NUMBER = reader.GetValue(4)?.ToString() ?? string.Empty;
                                var itemEMAIL = reader.GetValue(5)?.ToString() ?? string.Empty;
                                var itemPASSWORD = reader.GetValue(6)?.ToString() ?? string.Empty;
                                var itemROLE_NAME = reader.GetValue(7)?.ToString() ?? string.Empty;

                                // Validasi
                                if (string.IsNullOrWhiteSpace(itemACTION))
                                    itemMessage = "ACTION is required, ";
                                else if (string.IsNullOrWhiteSpace(itemFIRST_NAME))
                                    itemMessage = "First Name is required, ";
                                else if (string.IsNullOrWhiteSpace(itemLAST_NAME))
                                    itemMessage = "Last Name is required, ";
                                else if (string.IsNullOrWhiteSpace(itemPHONE_NUMBER))
                                    itemMessage = "Phone Number is required, ";
                                else if (string.IsNullOrWhiteSpace(itemEMAIL))
                                    itemMessage = "Email is required, ";
                                else if (string.IsNullOrWhiteSpace(itemROLE_NAME))
                                    itemMessage = "Role Name is required, ";
                                else if (users.Any(x => x.EMAIL == itemEMAIL))
                                    itemMessage = "Duplicate Email, ";
                                string hashedPassword = null;
                                if (itemACTION.ToUpper() == "ADD" && !string.IsNullOrWhiteSpace(itemPASSWORD))
                                {
                                    hashedPassword = PasswordHashing.Hash(itemPASSWORD);
                                }

                                users.Add(new UserUpload
                                {
                                    FIRST_NAME = itemFIRST_NAME,
                                    LAST_NAME = itemLAST_NAME,
                                    EMAIL = itemEMAIL,
                                    PHONE_NUMBER = itemPHONE_NUMBER,
                                    PASSWORD = hashedPassword,
                                    ROLE_NAME = itemROLE_NAME,
                                    MESSAGE = itemMessage,
                                    STATUS = string.IsNullOrEmpty(itemMessage) ? "Success" : "Error",
                                    NO = pos - step,
                                    ACTION = itemACTION.ToUpper()
                                });
                            }
                        }
                    }
                }

                if (users.Count == 0)
                    throw new Exception("Your File has no data");

                var result = UploadProcess(users, out string tempCode, out string msgError);

                response = new ExcelResponse
                {
                    tempCode = tempCode,
                    message = msgError,
                    status = result && string.IsNullOrEmpty(msgError) ? "Success" : "Error",
                    canSubmit = result && string.IsNullOrEmpty(msgError),
                    isSaved = true
                };

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                response = new ExcelResponse
                {
                    tempCode = string.Empty,
                    message = ex.Message,
                    status = "Error",
                    canSubmit = false,
                    isSaved = false
                };

                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return response;
        }
        public DataSourceResult GetUploadUserTemporary(DataSourceRequest request, string tempCode)
        {
            string methodName = "GetUploadUserTemporary";
            var result = new DataSourceResult();
            try
            {
                if (!string.IsNullOrEmpty(tempCode))
                {
                    request.Filters = request.Filters ?? new List<IFilterDescriptor>();
                    request.Filters.Add(new FilterDescriptor
                    {
                        Member = "TEMPCODE",
                        Operator = FilterOperator.IsEqualTo,
                        Value = tempCode
                    });
                }

                result = Db.UserRepository.GetUploadUserTemporary(request);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public bool UploadProcess(List<UserUpload> data, out string tempcode, out string msgerror)
        {
            bool result;
            string methodName = "UploadProcess";
            tempcode = string.Empty;

            try
            {
                string currentUser = SessionManager.Current;

                var uploadResult = Db.UserRepository.ValidateUploadProcess(data, currentUser, out tempcode, out msgerror);

                if (uploadResult > 0)
                {
                    result = true;
                }
                else
                {
                    throw new Exception(!string.IsNullOrEmpty(msgerror) ? msgerror : "Upload failed.");
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                msgerror = ex.Message;
                result = false;
            }

            return result;
        }
        public ExcelResponse SubmitExcelUpload(string tempCode)
        {
            const string methodName = "SubmitExcelUpload";

            try
            {
                string currentUser = SessionManager.Current;

                var submitResult = Db.UserRepository.SubmitFinalUpload(tempCode, currentUser, out string msgError);

                var response = new ExcelResponse
                {
                    tempCode = tempCode,
                    message = msgError,
                    status = submitResult > 0 && string.IsNullOrEmpty(msgError) ? "Success" : "Error",
                    isSaved = submitResult > 0
                };

                if (submitResult <= 0 && string.IsNullOrEmpty(msgError))
                {
                    response.message = "Submit upload failed.";
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
                return response;
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);

                return new ExcelResponse
                {
                    tempCode = tempCode,
                    message = ex.Message,
                    status = "Error",
                    isSaved = false
                };
            }
        }
        public List<UserUploadResponse> GetUserUploadValidationByTempCode(string tempCode)
        {
            string methodName = "GetUserUploadValidationByTempCode";
            var result = new List<UserUploadResponse>();
            try
            {
                result = Db.UserRepository.GetUserUploadValidationByTempCode(tempCode);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public UserView GetUserByPhoneEmail(string phoneNumber, string email)
        {
            var empty = string.Empty;
            return Db.UserRepository.ListUser("GetList", empty, phoneNumber, email).FirstOrDefault();
        }
        public UserView GetUserById(string id)
        {
            var empty = string.Empty;
            return Db.UserRepository.ListUser("GetList", id, empty, empty).FirstOrDefault();
        }
        public IEnumerable<string> GetRolesByUsername(int userId)
        {
            var userRole = Db.UserRoleRepository.Find(new { USER_ID = userId }).ToList();
            var userRoleString = userRole.Select(x => x.ROLE_NAME).ToList();

            return userRoleString.Count > 0 ? userRoleString : new[] { "User" }.ToList();
        }
        public List<string> GetUserRoleByUsername(int userId)
        {
            var userRole = Db.UserRoleRepository.Find(new { USER_ID = userId }).ToList();
            var userRoleString = userRole.Select(x => x.ROLE_NAME).ToList();
            return userRoleString;
        }
        public bool isSQLInjectString(string strWords)
        {
            strWords = Regex.Replace(strWords, @"[^\u0000-\u007F]+", string.Empty);

            bool isSQLInjection = false;
            List<string> badChars = new List<string>() { "select", "drop", ";", "--", "insert", "delete", "xp_", "¿", "waitfor delay" };

            for (int i = 0; i < badChars.Count(); i++)
            {
                if (strWords.Contains(badChars[i]))
                    isSQLInjection = true;
                break;
            }

            return isSQLInjection;
        }
        public BasicResponse Authenticate(LoginViewModel model)
        {
            var dataSourceService = new DataSourceService(Db);
            var (phoneNumber, email) = dataSourceService.ParseLoginInput(model.PhoneNumberOrEmail);
            string methodName = "Authenticate";
            string inputPassword = model.UserPwd;
            BasicResponse result = new();

            try
            {
                if (!isSQLInjectString(model.PhoneNumberOrEmail))
                {
                    var user = GetUserByPhoneEmail(phoneNumber, email) ?? throw new Exception("User or Password not match");

                    // user.PASSWORD sekarang berisi format {iterations}.{salt}.{hash}
                    bool valid = PasswordHashing.Verify(inputPassword, user.PASSWORD);

                    if (!valid)
                        throw new Exception("User or Password not match");
                }

                result.Success = true;
                //new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public TokenAuthenticatedResponse GetToken(UserView user)
        {
            var methodName = "GetToken";
            var result = new TokenAuthenticatedResponse();
            try
            {
                var roles = GetRolesByUsername(user.ID);

                var claims = new[]
                {
                    new Claim("Id", user.ID.ToString()),
                    new Claim("Name", user.FIRST_NAME),
                    new Claim("PhoneNumber", user.PHONE_NUMBER),
                    new Claim("Email", user.EMAIL),
                    new Claim("Roles", string.Join(",", roles)),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var authSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(GeneralHelper.Config?.JWT?.Secret ?? string.Empty));

                var jwtToken = new JwtSecurityToken(
                    issuer: GeneralHelper.Config?.JWT?.ValidIssuer,
                    audience: GeneralHelper.Config?.JWT?.ValidAudience,
                    expires: DateTime.Now.AddHours(GeneralHelper.Config?.JWT?.ExpireHour ?? 24),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                var validTo = jwtToken.ValidTo.ToLocalTime();

                var logData = new AuthenticationLog()
                {
                    USER_ID = user.ID,
                    TOKEN = token,
                    VALID_TO = validTo,
                    CREATED_BY = user.FIRST_NAME,
                    CREATED_DATE = DateTime.Now
                };

                var addStatus = Db.AuthenticationLogRepository.Create(logData, new string[] {
                    "USER_ID",
                    "TOKEN",
                    "VALID_TO",
                    "CREATED_BY",
                    "CREATED_DATE"
                });

                if (addStatus <= 0)
                {
                    throw new Exception("Token Generate Failed");
                }
                else
                {
                    result.Success = true;
                    result.Token = token;
                    result.ValidTo = validTo;
                    //new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        private void RecurseFilterDescriptors(IList<Kendo.Mvc.IFilterDescriptor> requestFilters, List<Kendo.Mvc.FilterDescriptor> allFilterDescriptors)
        {
            foreach (var filterDescriptor in requestFilters)
            {
                if (filterDescriptor is Kendo.Mvc.FilterDescriptor)
                {
                    allFilterDescriptors.Add((Kendo.Mvc.FilterDescriptor)filterDescriptor);
                }
                else if (filterDescriptor is Kendo.Mvc.CompositeFilterDescriptor)
                {
                    RecurseFilterDescriptors(((Kendo.Mvc.CompositeFilterDescriptor)filterDescriptor).FilterDescriptors, allFilterDescriptors);
                }
            }
        }
        private string GenerateToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
        private string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }
    }
    public class ModelException : Exception
    {
        public string Title { get; }
        public ModelException(string title, string message)
            : base(message)
        {
            Title = title;
            this.Data.Add("Title", title);
        }
    }

}
