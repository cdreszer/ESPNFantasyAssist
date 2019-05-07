# ESPNFantasyAssist
C# Console program that scrapes ESPN Fantasy Baseball Team to create an active and positional stats page.<br />
Optional commandline argument to pass path to HTML files to specify which files to use... ex: "./HTMLFiles/", "./HTMLFiles/Dad" <br />
  &nbsp;&nbsp; **- Run ./scrape.sh in PyESPNScraper folder to: **<br />
  &nbsp;&nbsp; ** scrape espn, build fantasy team, and update google doc spreadsheet. **<br />

#### **HTML retrieval:** <br />
  Currently using Python/Selenium to retrieve HTML files from ESPN.<br />
  Using selenium in order to get the HTML after javascript manipulation.<br />


#### **HTMLParser:**<br />
  Uses CSS classes to retrive the player names and stat values for a given name.<br />
  At this point is the majority of the code in order to create FantasyTeam.<br />
  <br />
#### **Model:**<br />
  * *FantasyTeam*<br />
    * Team name<br />
    * List of batters<br />
    * List of pitchers<br />
    * Map of stats by batter position<br />
    * Last day updated<br />
    <br />
  (May decide to create seperate Batter/Pitcher classes that extend Player)<br />
  * *Player*<br />
    * id<br />
    * name/position/team<br />
    * isBatter (true or false)<br />
    * Map of active stats<br />
    * Map of inactive stats<br />
    * List of weekly totals (map of stats)<br />
    * Last day updated<br />
    <br />
  * *Util*<br />
    * Calculations<br />
    * Additional useful maps, category arrays<br />
    <br />
  * *Program* <br />
    * Simple driver for console app.<br />
  
#### **SpreadsheetGenerator:**<br />
  Google Doc Spreadsheet Generator that programmatically updates a google doc spreadsheet with up to date stats retrieved. (using Google Spreadsheets API)<br />

