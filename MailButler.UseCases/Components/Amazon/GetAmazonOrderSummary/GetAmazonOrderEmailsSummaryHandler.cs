using System.Text;
using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.Components.Amazon.GetAmazonOrderSummary;

public class GetAmazonOrderEmailsSummaryHandler : IRequestHandler<GetAmazonOrderEmailsSummaryRequest,
	GetAmazonOrderEmailsSummaryResponse>
{
	private const string EmailId = "edae6e37-91c6-4103-b779-ad800107ad1c";

	private const string MiniconSvgImage =
		"<svg version=\"1.0\" id=\"logo\" xmlns=\"http://www.w3.org/2000/svg\"\n width=\"65.000000pt\" height=\"67.000000pt\" viewBox=\"0 0 652.000000 673.000000\"\n preserveAspectRatio=\"xMidYMid meet\"><g transform=\"translate(0.000000,673.000000) scale(0.100000,-0.100000)\"\nfill=\"#000000\" stroke=\"none\">\n<path d=\"M0 3375 l0 -3345 3260 0 3260 0 0 3345 0 3345 -3260 0 -3260 0 0\n-3345z m6290 3220 c16 -8 35 -15 43 -15 7 0 24 -15 38 -32 l24 -33 3 -3134 c1\n-2088 -1 -3146 -8 -3170 -6 -20 -19 -47 -29 -61 l-20 -25 -3075 0 c-3065 0\n-3075 0 -3101 20 -15 11 -36 42 -46 68 -19 47 -19 136 -19 3177 0 1965 4 3138\n10 3154 5 14 21 33 37 43 26 17 93 18 1233 23 663 3 2032 4 3043 3 1612 -3\n1842 -5 1867 -18z\"/>\n<path d=\"M4925 5934 c-38 -9 -99 -22 -135 -30 -36 -8 -74 -14 -85 -14 -11 0\n-49 -6 -85 -14 -36 -8 -103 -22 -150 -31 -47 -9 -112 -23 -145 -30 -33 -8\n-121 -26 -195 -39 -74 -14 -156 -30 -181 -36 -25 -6 -59 -13 -75 -15 -16 -2\n-54 -9 -84 -15 -30 -6 -68 -14 -85 -16 -16 -3 -43 -9 -60 -13 -16 -5 -75 -16\n-130 -26 -55 -9 -127 -23 -160 -30 -82 -19 -189 -41 -255 -54 -187 -35 -545\n-105 -600 -117 -36 -7 -90 -18 -120 -24 -30 -6 -113 -22 -185 -36 -71 -15\n-153 -30 -182 -34 -28 -5 -56 -11 -61 -14 -5 -3 -43 -10 -83 -16 -40 -6 -108\n-19 -150 -30 -42 -10 -96 -21 -120 -25 -24 -3 -71 -12 -104 -20 -33 -7 -123\n-26 -200 -40 -77 -15 -167 -33 -200 -40 -33 -8 -78 -17 -100 -21 -22 -3 -98\n-19 -170 -35 -174 -40 -199 -65 -184 -188 10 -76 104 -561 134 -691 13 -58 31\n-148 41 -200 9 -52 32 -171 51 -265 19 -93 48 -235 65 -315 16 -80 33 -170 38\n-200 5 -30 18 -100 29 -155 12 -55 28 -136 37 -180 9 -44 22 -102 29 -130 7\n-27 21 -93 31 -145 9 -52 28 -149 41 -215 13 -66 35 -185 49 -265 13 -80 39\n-212 56 -295 17 -82 41 -205 54 -272 12 -68 25 -126 28 -131 3 -4 12 -53 21\n-108 14 -92 54 -294 90 -459 8 -38 15 -77 15 -85 0 -8 5 -32 11 -53 9 -35 13\n-38 42 -35 28 3 32 0 30 -19 -3 -23 20 -48 36 -39 6 4 7 14 4 22 -5 13 -1 15\n16 9 19 -6 21 -4 17 17 -3 13 -15 75 -26 138 -33 179 -50 269 -66 340 -36 168\n-46 216 -74 365 -17 88 -35 174 -40 190 -5 17 -12 50 -15 75 -3 25 -9 65 -15\n90 -83 384 -163 779 -195 965 -25 149 -46 248 -58 270 -6 11 -12 36 -13 55 -1\n19 -19 114 -40 210 -20 96 -40 200 -44 230 -7 55 -19 125 -45 250 -8 36 -21\n101 -30 145 -9 44 -21 96 -27 115 -6 19 -15 64 -19 100 -10 76 -13 90 -68 360\n-60 299 -61 303 -67 327 -7 27 14 37 109 53 91 15 193 34 362 68 174 36 368\n72 407 77 15 2 44 8 65 14 21 5 74 17 118 25 44 9 132 27 195 40 63 14 130 27\n148 29 18 3 54 10 80 16 26 6 99 20 162 32 63 11 147 29 187 39 40 11 88 22\n107 24 19 2 59 9 88 15 29 6 109 22 178 36 69 14 172 34 230 46 58 11 128 24\n155 29 28 6 97 19 155 31 58 11 121 24 140 29 19 5 49 11 65 13 56 8 260 48\n320 62 33 8 112 24 175 36 63 12 122 25 130 29 8 4 40 11 70 14 30 4 71 11 90\n16 19 5 53 11 75 15 22 3 47 8 55 10 14 4 57 13 260 50 41 8 92 18 113 24 20\n5 38 6 39 2 0 -3 2 -17 3 -31 1 -14 12 -68 24 -120 28 -119 37 -160 61 -285\n11 -55 34 -161 51 -235 16 -74 41 -193 54 -265 14 -71 31 -161 39 -200 8 -38\n20 -99 26 -135 7 -36 23 -119 37 -185 14 -66 29 -142 33 -170 5 -27 18 -95 30\n-150 11 -55 28 -138 36 -185 8 -47 21 -116 29 -155 16 -75 63 -308 95 -470 11\n-55 24 -125 31 -155 6 -30 33 -167 60 -303 27 -136 51 -260 54 -275 3 -15 12\n-61 20 -102 8 -41 21 -109 30 -150 8 -41 22 -109 30 -150 8 -41 21 -106 29\n-145 8 -38 19 -97 26 -130 6 -33 15 -82 20 -110 10 -49 19 -111 23 -149 2 -17\n-26 -24 -268 -72 -148 -30 -317 -63 -375 -75 -58 -12 -150 -30 -205 -40 -99\n-20 -137 -28 -225 -48 -25 -6 -130 -27 -235 -47 -104 -19 -217 -42 -250 -49\n-33 -8 -231 -48 -440 -90 -209 -41 -432 -86 -495 -99 -63 -14 -185 -38 -270\n-55 -85 -18 -186 -38 -225 -46 -38 -8 -108 -22 -155 -30 -95 -16 -166 -30\n-255 -50 -63 -15 -309 -62 -360 -69 -16 -2 -64 -12 -105 -21 -75 -17 -184 -37\n-295 -56 -103 -17 -111 -19 -94 -30 7 -5 32 -9 54 -9 23 0 53 -9 70 -20 19\n-13 47 -20 77 -20 26 0 50 -5 54 -11 4 -7 20 -9 38 -6 163 30 194 36 240 48\n21 6 62 14 90 19 28 5 105 21 171 35 66 14 156 32 200 40 76 13 182 35 340 70\n39 9 117 24 175 35 258 47 316 59 330 64 8 3 65 15 125 25 61 10 187 35 280\n54 94 20 211 43 260 52 152 27 248 46 295 56 24 6 101 21 170 34 69 13 157 31\n195 40 39 9 108 23 154 31 46 8 87 16 91 19 4 3 32 7 61 11 30 3 58 9 63 12 5\n3 50 11 100 18 50 8 136 23 191 35 55 11 132 27 170 34 332 64 352 70 381 127\n20 39 17 87 -11 223 -14 66 -30 145 -36 175 -5 30 -21 105 -34 165 -34 154\n-192 942 -210 1045 -14 80 -27 144 -61 300 -26 124 -137 675 -149 745 -23 132\n-37 200 -45 230 -8 25 -31 144 -46 235 -3 14 -9 39 -14 55 -5 17 -19 82 -30\n145 -12 63 -30 151 -41 195 -10 44 -23 108 -28 143 -6 34 -12 66 -15 71 -3 5\n-10 42 -16 82 -5 41 -15 94 -20 119 -6 25 -17 90 -26 145 -26 165 -51 281 -70\n320 -33 71 -96 87 -229 59z\"/>\n<path d=\"M3231 5293 c-17 -71 -32 -103 -46 -103 -31 0 -45 -30 -41 -91 3 -45\n-3 -80 -23 -142 -46 -143 -22 -152 -261 91 -115 116 -214 212 -220 212 -22 0\n-207 -73 -307 -121 -91 -43 -287 -155 -320 -181 -10 -8 -3 -22 30 -60 23 -27\n51 -65 62 -83 11 -18 63 -77 116 -131 97 -100 199 -224 199 -244 0 -6 33 -43\n73 -83 189 -191 345 -354 359 -378 31 -49 320 -350 346 -360 21 -8 29 -7 39 8\n8 10 20 26 28 36 8 9 13 17 10 17 -3 0 5 18 16 40 12 23 24 39 26 36 3 -2 0\n-13 -6 -24 -7 -13 -8 -29 -2 -44 7 -20 13 -13 50 63 22 47 41 89 41 92 0 4 11\n24 24 45 14 20 47 89 74 152 27 64 68 156 92 205 44 93 132 291 180 405 29 69\n60 139 90 205 23 49 150 347 150 351 0 1 -17 8 -37 14 -21 6 -47 15 -58 20\n-103 44 -422 100 -571 100 l-102 0 -11 -47z\"/>\n<path d=\"M4101 5128 c19 -68 28 -149 39 -338 19 -359 20 -385 26 -455 4 -38\n11 -76 17 -83 8 -10 8 -15 0 -20 -8 -5 -7 -84 3 -282 7 -151 18 -376 24 -500\n11 -245 34 -670 50 -930 22 -359 33 -566 46 -875 4 -82 8 -157 10 -165 1 -8 3\n-19 3 -24 1 -5 9 -2 19 7 10 10 22 17 26 17 5 0 22 12 40 26 17 14 73 57 124\n95 181 138 402 378 493 536 l42 74 -17 297 c-9 163 -23 396 -31 517 -30 437\n-37 552 -40 660 -2 61 -6 148 -9 195 -3 47 -10 200 -17 340 -9 191 -16 264\n-28 290 -18 39 -311 342 -368 380 -21 14 -53 39 -73 56 -62 54 -346 214 -379\n214 -6 0 -6 -12 0 -32z\"/>\n<path d=\"M1911 4888 c-70 -53 -215 -195 -278 -273 -55 -67 -163 -217 -184\n-257 -41 -74 -159 -321 -159 -332 0 -7 -8 -24 -19 -37 -42 -55 -45 -103 -27\n-465 26 -526 55 -1046 61 -1077 7 -41 107 -234 172 -335 59 -89 185 -247 270\n-339 101 -108 402 -338 420 -320 3 4 1 77 -5 162 -6 86 -9 161 -6 168 2 7 -2\n77 -9 157 -7 80 -11 189 -9 243 3 90 5 99 25 106 17 6 34 -2 82 -38 109 -84\n185 -133 210 -135 13 -2 74 -48 142 -107 65 -58 156 -134 203 -171 47 -36 132\n-108 190 -159 164 -146 457 -389 468 -389 6 0 12 -4 14 -9 2 -5 28 -28 58 -51\n53 -40 57 -41 110 -35 108 12 268 62 408 127 46 21 87 38 92 38 22 0 170 84\n163 91 -5 5 -47 39 -95 76 -47 38 -140 113 -206 168 -66 55 -150 123 -186 150\n-111 83 -528 418 -746 599 -113 93 -227 188 -255 210 -27 23 -61 51 -75 62\n-231 197 -563 469 -617 505 l-61 42 -6 91 c-4 50 -11 190 -16 311 -5 121 -14\n285 -19 365 -5 80 -12 224 -15 320 -5 153 -11 273 -21 400 -1 19 -3 68 -4 108\n0 39 -4 72 -8 71 -5 0 -32 -19 -62 -41z\"/>\n</g>\n</svg>";

	private readonly IList<Account> _accounts;

	public GetAmazonOrderEmailsSummaryHandler(IList<Account> accounts)
	{
		_accounts = accounts;
	}

	public Task<GetAmazonOrderEmailsSummaryResponse> Handle(GetAmazonOrderEmailsSummaryRequest request,
		CancellationToken cancellationToken)
	{
		if (request.EmailsWithOrders.Keys.Count == 0)
			return Task.FromResult(new GetAmazonOrderEmailsSummaryResponse
			{
				Result = new Email
				{
					Subject = "Summary of Amazon Orders",
					TextBody = "No emails found with Amazon orders",
					HtmlBody = HtmlLogo() + "<p>No emails found with Amazon orders</p>"
				}
			});

		if (request.EmailsWithOrders.Keys.All(email => email.IsRead))
			return Task.FromResult(
				new GetAmazonOrderEmailsSummaryResponse
				{
					Result = new Email
					{
						Sender = new MailBoxAddress
						{
							Name = "MailButler",
							Address = "mailbutler@minicon.eu"
						},
						Subject =
							$"Summary of Amazon Orders Since: {request.EmailsWithOrders.Min(email => email.Key.Sent):yyyy-MM-dd}",
						TextBody = "All emails are already read",
						HtmlBody = HtmlLogo() + "<p>No new emails with Amazon orders</p>"
					}
				}
			);

		return Task.FromResult(
			new GetAmazonOrderEmailsSummaryResponse
			{
				Result = SummaryEmail(request.EmailsWithOrders)
			}
		);
	}

	private Email SummaryEmail(Dictionary<Email, List<string>> emailsWithOrders)
	{
		return new Email
		{
			Sender = new MailBoxAddress
			{
				Name = "MailButler",
				Address = "mailbutler@minicon.eu"
			},
			TextBody = TextBody(emailsWithOrders),
			HtmlBody = HtmlBody(emailsWithOrders),
			Subject =
				$"Summary of Amazon Orders Emails since: {emailsWithOrders.Min(email => email.Key.Sent):yyyy-MM-dd}"
		};
	}

	public string HtmlLogo()
	{
		StringBuilder htmlBody = new();
		htmlBody.AppendLine("<center id='logo'>");
		htmlBody.AppendLine(MiniconSvgImage);
		htmlBody.AppendLine("<h1>MailButler - Amazon Summary</h1>");
		htmlBody.AppendLine("</center>");
		return htmlBody.ToString();
	}

	private string HtmlStyle()
	{
		StringBuilder htmlStyle = new();
		htmlStyle.AppendLine("<style>");
		htmlStyle.AppendLine("body {");
		htmlStyle.AppendLine("font-family: Arial, Helvetica, sans-serif;");
		htmlStyle.AppendLine("}");
		htmlStyle.AppendLine("#logo {");
		htmlStyle.AppendLine("margin-top: 2em;");
		htmlStyle.AppendLine("margin-bottom: 1em;");
		htmlStyle.AppendLine("}");
		htmlStyle.AppendLine("h1 {");
		htmlStyle.AppendLine("font-size: 4em;");
		htmlStyle.AppendLine("}");
		htmlStyle.AppendLine("h2 {");
		htmlStyle.AppendLine("font-size: 3em;");
		htmlStyle.AppendLine("margin-top: 1.5em;");
		htmlStyle.AppendLine("margin-bottom: 1.0em;");
		htmlStyle.AppendLine("}");
		htmlStyle.AppendLine("h3 {");
		htmlStyle.AppendLine("font-size: 2em;");
		htmlStyle.AppendLine("margin-top: 1.0em;");
		htmlStyle.AppendLine("margin-bottom: 0.75em;");
		htmlStyle.AppendLine("}");
		htmlStyle.AppendLine("</style>");
		return htmlStyle.ToString();
	}

	private string HtmlBody(Dictionary<Email, List<string>> emailsWithOrders)
	{
		StringBuilder htmlBody = new(HtmlStyle());
		htmlBody.AppendLine(HtmlLogo());

		var accounts = _accounts.ToDictionary(e => e.Id, account => account);

		SellerEmailsHtmlSummary(emailsWithOrders, htmlBody);
		OrderEmailsHtmlSummary(emailsWithOrders, htmlBody, accounts);

		htmlBody.AppendLine($"<!-- Id: {EmailId} -->");
		return htmlBody.ToString();
	}

	private static void SellerEmailsHtmlSummary(Dictionary<Email, List<string>> emailsWithOrders,
		StringBuilder htmlBody)
	{
		var sellerEmails = UnreadSellerEmails(emailsWithOrders);
		if (sellerEmails.All(e => e.Key.IsRead))
			return;

		int ordersInSellerEmails = sellerEmails.Sum(ite => ite.Value.Count);
		htmlBody.AppendLine($"<h2>Sold on Amazon: <i>{ordersInSellerEmails}</i></h2>");
		foreach (var sellerEmail in sellerEmails.GroupBy(e => e.Key.Subject))
		{
			htmlBody.AppendLine("<h3>");
			htmlBody.Append($"{sellerEmail.Key}");
			if (sellerEmail.Count() > 1)
			{
				htmlBody.AppendLine($" <i>({sellerEmail.Sum(ite => ite.Value.Count)})</i>");
			}

			htmlBody.AppendLine($"</h3>");

			if (sellerEmail.Count() > 1)
			{
				htmlBody.AppendLine("<ol>");
				htmlBody.AppendJoin(
					"\r\n",
					sellerEmail
						.SelectMany(item => item.Value)
						.Select(item => $" <li>{item}</li>")
				);
				htmlBody.AppendLine("</ol>");
			}

			if (sellerEmail.Count() == 1)
			{
				htmlBody.AppendLine(sellerEmail
					.SelectMany(item => item.Value)
					.Select(item => $" <li>{item}</li>")
					.Single()
				);
			}
		}

		htmlBody.AppendLine();
	}

	private static void OrderEmailsHtmlSummary(Dictionary<Email, List<string>> emailsWithOrders, StringBuilder htmlBody,
		Dictionary<Guid, Account> accounts)
	{
		List<string> ordersEmails = emailsWithOrders
			.Where(item => !IsAmazonSellerEmail(item))
			.OrderByDescending(e => e.Key.Sent)
			.SelectMany(e => e.Value)
			.Distinct()
			.ToList();

		if (!ordersEmails.Any())
			return;

		htmlBody.AppendLine($"<h2>Orders:</h2>");
		foreach (var order in ordersEmails)
		{
			var orderEmails = emailsWithOrders
				.Where(e => e.Value.Contains(order))
				.OrderByDescending(e => e.Key.Sent)
				.ToList();

			if (orderEmails.All(e => e.Key.IsRead))
			{
				continue;
			}

			htmlBody.AppendLine($"<h3>{order} <i>({accounts[orderEmails.First().Key.AccountId].Name})</i>:</h3>");
			if (orderEmails.Count > 1)
			{
				htmlBody.AppendLine("<ol>");
			}

			htmlBody.AppendJoin(
				"\r\n",
				orderEmails
					.Select(
						email
							=>
							$"<li>{(email.Key.IsRead ? "" : "*")}{email.Key.Sent:yyyy-MM-dd}: {email.Key.Subject}</li>"
					)
			);
			if (orderEmails.Count >= 1)
			{
				htmlBody.AppendLine("</ol>");
			}
		}
	}

	private string TextBody(Dictionary<Email, List<string>> emailsWithOrders)
	{
		StringBuilder textBody = new();
		var accounts = _accounts.ToDictionary(e => e.Id, account => account);

		SellerEmailsTextSummary(emailsWithOrders, textBody);
		OrderEmailsTextSummary(emailsWithOrders, textBody, accounts);

		textBody.AppendLine("r\n\r\n\r\n\r\n");
		textBody.AppendLine($"Id: {EmailId}");
		return textBody.ToString();
	}

	private static void OrderEmailsTextSummary(Dictionary<Email, List<string>> emailsWithOrders, StringBuilder textBody,
		Dictionary<Guid, Account> accounts)
	{
		if (!emailsWithOrders.Any())
			return;

		textBody.AppendLine("Orders: ");

		foreach (var order in emailsWithOrders
			         .Where(item => !IsAmazonSellerEmail(item))
			         .OrderByDescending(e => e.Key.Sent)
			         .SelectMany(e => e.Value)
			         .Distinct()
		        )
		{
			HashSet<string> lines = new();
			var orderEmails = emailsWithOrders
				.Where(e => e.Value.Contains(order))
				.OrderByDescending(e => e.Key.Sent)
				.ToList();

			textBody.AppendLine($"{order} ({accounts[orderEmails.First().Key.AccountId].Name}):");

			foreach (var email in orderEmails)
				lines.Add($"- {(email.Key.IsRead ? "" : "*")}{email.Key.Sent:yyyy-MM-dd}: {email.Key.Subject}");

			textBody.AppendJoin("\r\n", lines);
			textBody.AppendLine();
		}
	}

	private static void SellerEmailsTextSummary(Dictionary<Email, List<string>> emailsWithOrders,
		StringBuilder textBody)
	{
		var sellerEmails = UnreadSellerEmails(emailsWithOrders);
		if (sellerEmails.Any(e => !e.Key.IsRead))
		{
			var ordersInSellerEmails = sellerEmails.Sum(ite => ite.Value.Count);
			textBody.AppendLine($"Sold on Amazon: {ordersInSellerEmails}");
			foreach (var sellerEmail in sellerEmails.GroupBy(e => e.Key.Subject))
			{
				textBody.AppendLine($" {sellerEmail.Key}: {sellerEmail.Sum(ite => ite.Value.Count)}");
				textBody.AppendJoin(
					"\r\n",
					sellerEmail
						.SelectMany(item => item.Value)
						.Select(item => $" - {item}")
				);
			}

			textBody.AppendLine();
		}
	}

	private static bool IsAmazonSellerEmail(KeyValuePair<Email, List<string>> email)
	{
		return email.Key.Sender.Address == "donotreply@amazon.com";
	}

	private static List<KeyValuePair<Email, List<string>>> UnreadSellerEmails(
		Dictionary<Email, List<string>> emailsWithOrders)
	{
		return emailsWithOrders
			.Where(item => IsAmazonSellerEmail(item))
			.Where(item => !item.Key.IsRead)
			.OrderByDescending(e => e.Key.Sent)
			.ToList();
	}
}