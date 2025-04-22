using System;

namespace Common.Template
{
    public static class EmailTemplate
    {
        public const string logoUrl = "https://i.pinimg.com/736x/e6/b2/f2/e6b2f20319b49615c698f143dc8a2aa7.jpg"; // Thay thế bằng URL logo thực tế của Speak AI

        public static string OTPEmailTemplate(string userName, string otpCode, string subject)
        {
            string htmlTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{TITLE}</title>
    <style>
        body {
            font-family: 'Arial', sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
            color: #333;
        }
        .email-container {
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }
        .header {
            background-color: #4a90e2;
            color: white;
            padding: 20px;
            text-align: center;
        }
        .header img {
            max-width: 150px;
        }
        .content {
            padding: 20px;
            text-align: center;
        }
        .otp-code {
            font-size: 32px;
            font-weight: bold;
            color: #4a90e2;
            margin: 20px 0;
        }
        .footer {
            background-color: #f4f4f9;
            padding: 10px;
            text-align: center;
            font-size: 12px;
            color: #777;
        }
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <img src=""{LOGO_URL}"" alt=""Speak AI Logo"">
            <h1>Welcome to Speak AI</h1>
        </div>
        <div class=""content"">
            <p>Hello {USER_NAME},</p>
            <p>Thank you for choosing Speak AI to enhance your English skills. Here is your OTP code to verify your account:</p>
            <div class=""otp-code"">{OTP_CODE}</div>
            <p>This code will expire in 15 minutes. Please do not share it with anyone.</p>
        </div>
        <div class=""footer"">
            <p>This is an automated email, please do not reply.</p>
            <p>Speak AI - Mastering English, One Word at a Time.</p>
        </div>
    </div>
</body>
</html>";

            htmlTemplate = htmlTemplate
                .Replace("{OTP_CODE}", otpCode)
                .Replace("{USER_NAME}", userName)
                .Replace("{LOGO_URL}", logoUrl)
                .Replace("{TITLE}", subject);

            return htmlTemplate;
        }

        public static string WelcomeEmailTemplate(string userName, string subject)
        {
            string htmlTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{TITLE}</title>
    <style>
        body {
            font-family: 'Arial', sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
            color: #333;
        }
        .email-container {
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }
        .header {
            background-color: #4a90e2;
            color: white;
            padding: 20px;
            text-align: center;
        }
        .header img {
            max-width: 150px;
        }
        .content {
            padding: 20px;
            text-align: center;
        }
        .footer {
            background-color: #f4f4f9;
            padding: 10px;
            text-align: center;
            font-size: 12px;
            color: #777;
        }
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <img src=""{LOGO_URL}"" alt=""Speak AI Logo"">
            <h1>Welcome to Speak AI</h1>
        </div>
        <div class=""content"">
            <p>Hello {USER_NAME},</p>
            <p>Welcome to Speak AI, your new partner in mastering the English language! We're thrilled to have you on board and can't wait to see your progress.</p>
            <p>With Speak AI, you'll have access to a variety of tools and resources designed to improve your English skills effectively and enjoyably.</p>
            <p>Start your journey today and explore all the features we offer:</p>
            <ul>
                <li>Interactive lessons tailored to your level</li>
                <li>Real-time pronunciation feedback</li>
                <li>Engaging quizzes and games</li>
            </ul>
            <p>If you have any questions or need assistance, feel free to reach out to our support team.</p>
            <p>Happy learning!</p>
        </div>
        <div class=""footer"">
            <p>This is an automated email, please do not reply.</p>
            <p>Speak AI - Mastering English, One Word at a Time.</p>
        </div>
    </div>
</body>
</html>";

            htmlTemplate = htmlTemplate
                .Replace("{USER_NAME}", userName)
                .Replace("{LOGO_URL}", logoUrl)
                .Replace("{TITLE}", subject);

            return htmlTemplate;
        }
        public static string PremiumUpgradeTemplate(string userName, string subject)
        {
            string htmlTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{TITLE}</title>
    <style>
        body {
            font-family: 'Arial', sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
            color: #333;
        }
        .email-container {
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }
        .header {
            background-color: #4a90e2;
            color: white;
            padding: 20px;
            text-align: center;
        }
        .header img {
            max-width: 150px;
        }
        .content {
            padding: 20px;
            text-align: center;
        }
        .premium-badge {
            font-size: 24px;
            color: #ffd700;
            margin: 20px 0;
        }
        .features-list {
            text-align: left;
            margin: 20px 40px;
        }
        .footer {
            background-color: #f4f4f9;
            padding: 10px;
            text-align: center;
            font-size: 12px;
            color: #777;
        }
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <img src=""{LOGO_URL}"" alt=""Speak AI Logo"">
            <h1>Premium Membership Activated</h1>
        </div>
        <div class=""content"">
            <p>Dear {USER_NAME},</p>
            <div class=""premium-badge"">🌟 Welcome to Premium! 🌟</div>
            <p>Congratulations! Your account has been successfully upgraded to Premium status.</p>
            <p>Enjoy these exclusive Premium benefits:</p>
            <div class=""features-list"">
                <ul>
                    <li>Access to all premium courses</li>
                    <li>Advanced pronunciation exercises</li>
                    <li>Personalized learning path</li>
                    <li>Priority customer support</li>
                    <li>Ad-free experience</li>
                    <li>Download learning materials</li>
                </ul>
            </div>
            <p>Start exploring your premium features now and take your English learning to the next level!</p>
            <p>Thank you for choosing Speak AI Premium.</p>
        </div>
        <div class=""footer"">
            <p>This is an automated email, please do not reply.</p>
            <p>Speak AI - Mastering English, One Word at a Time.</p>
        </div>
    </div>
</body>
</html>";

            htmlTemplate = htmlTemplate
                .Replace("{USER_NAME}", userName)
                .Replace("{LOGO_URL}", logoUrl)
                .Replace("{TITLE}", subject);

            return htmlTemplate;
        }

        public static string PremiumReceiptTemplate(string userName, decimal amount, string orderId, string subject)
        {
            string htmlTemplate = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{TITLE}</title>
    <style>
        body {
            font-family: 'Arial', sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
            color: #333;
        }
        .email-container {
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }
        .header {
            background-color: #4a90e2;
            color: white;
            padding: 20px;
            text-align: center;
        }
        .header img {
            max-width: 150px;
        }
        .content {
            padding: 20px;
            text-align: center;
        }
        .receipt-details {
            background-color: #f8f9fa;
            margin: 20px;
            padding: 20px;
            border-radius: 4px;
            text-align: left;
        }
        .receipt-row {
            display: flex;
            justify-content: space-between;
            margin: 10px 0;
            border-bottom: 1px solid #eee;
            padding-bottom: 5px;
        }
        .footer {
            background-color: #f4f4f9;
            padding: 10px;
            text-align: center;
            font-size: 12px;
            color: #777;
        }
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <img src=""{LOGO_URL}"" alt=""Speak AI Logo"">
            <h1>Payment Receipt</h1>
        </div>
        <div class=""content"">
            <p>Dear {USER_NAME},</p>
            <p>Thank you for your purchase! Here's your receipt for Premium membership:</p>
            <div class=""receipt-details"">
                <div class=""receipt-row"">
                    <span>Order ID: </span>
                    <span>{ORDER_ID}</span>
                </div>
                <div class=""receipt-row"">
                    <span>Date: </span>
                    <span>{DATE}</span>
                </div>
                <div class=""receipt-row"">
                    <span>Amount Paid:</span>
                    <span>{AMOUNT} VND</span>
                </div>
                <div class=""receipt-row"">
                    <span>Payment Status: </span>
                    <span>Completed</span>
                </div>
            </div>
            <p>If you have any questions about your purchase, please contact our support team.</p>
        </div>
        <div class=""footer"">
            <p>This is an automated email, please do not reply.</p>
            <p>Speak AI - Mastering English, One Word at a Time.</p>
        </div>
    </div>
</body>
</html>";

            htmlTemplate = htmlTemplate
                .Replace("{USER_NAME}", userName)
                .Replace("{LOGO_URL}", logoUrl)
                .Replace("{TITLE}", subject)
                .Replace("{ORDER_ID}", orderId)
                .Replace("{AMOUNT}", amount.ToString("0.00"))
                .Replace("{DATE}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            return htmlTemplate;
        }
    }
}