using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ESPNFantasyAssist
{
    static class SpreadsheetGenerator
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        static string spreadsheetId = "1DB3CNvXUxtK8S7ZZwKHaQ177CV1XqFAOeTK5TdhJqUs";
        static string date = DateTime.Today.ToString();

        // Maps html file path to corresponding sheet destination.
        static Dictionary<string, string> SpreadSheetNameByPath = new Dictionary<string, string>() {
                {"./HTMLFiles/", "Sheet2"},
                {"./HTMLFiles/Dad/", "Sheet1"},
                {"./HTMLFiles/Miles/", "Sheet3"}
        };

        /// Inserts the fantasy team into the correct google docs sheet
        public static void ExportToGoogleDocsSpreadsheet(string path, FantasyTeam team) {
            // Authorizes google sheets api service
            var service = GoogleSheetsAPIAuthorization();

            int row = 1;
            // Writes batters active stats
            List<IList<object>> data = new List<IList<object>>();

            data.Add(new List<object>(){"LAST UPDATED: " + date});
            data.Add(new List<object>(){"ACTIVE BATTERS"});
            data.AddRange(team.GetStatsAsListOfLists(team.Batters, true, true));
            data.AddRange(team.GetTotalAsListOfLists(team.Batters, true, true));

            data.Add(new List<object>(){""});
            data.Add(new List<object>(){"ACTIVE PITCHERS"});
            data.AddRange(team.GetStatsAsListOfLists(team.Pitchers, true, false));
            data.AddRange(team.GetTotalAsListOfLists(team.Pitchers, true, false));

            data.Add(new List<object>(){""});
            data.Add(new List<object>(){"INACTIVE BATTERS"});
            data.AddRange(team.GetStatsAsListOfLists(team.Batters, false, true));
            data.AddRange(team.GetTotalAsListOfLists(team.Batters, false, true));

            data.Add(new List<object>(){""});
            data.Add(new List<object>(){"INACTIVE PITCHERS"});
            data.AddRange(team.GetStatsAsListOfLists(team.Pitchers, false, false));
            data.AddRange(team.GetTotalAsListOfLists(team.Pitchers, false, false));

            data.Add(new List<object>(){""});
            data.Add(new List<object>(){"STATS BY POSTION"});
            data.AddRange(team.GetStatsByPositionAsListOfLists());
            data.AddRange(team.GetTotalAsListOfLists(team.Batters, true, true));

            InsertData(service, SpreadSheetNameByPath[path], row, data);
        }

        /// Inserts the data into specific sheet name at row # provided
        private static void InsertData(SheetsService service, string sheetName, int row, List<IList<object>> data) {
            String range = sheetName + "!A" + row + ":Y";
            string valueInputOption = "USER_ENTERED";

            // The new values to apply to the spreadsheet.
            List<Google.Apis.Sheets.v4.Data.ValueRange> updateData = new List<Google.Apis.Sheets.v4.Data.ValueRange>();
            var dataValueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            dataValueRange.Range = range;
            dataValueRange.Values = data;
            updateData.Add(dataValueRange);

            Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest();
            requestBody.ValueInputOption = valueInputOption;
            requestBody.Data = updateData;

            var request = service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse response = request.Execute();
        }

        /// Authorizes google sheets api connection
        private static SheetsService GoogleSheetsAPIAuthorization() {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return service;
        }
    }
}
