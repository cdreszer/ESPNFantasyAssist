# ESPNFantasyAssist
C# Console program that scrapes ESPN Fantasy Baseball Team to create an active and positional stats page.<br />

#### **HTML retrieval:** <br />
  Currently using Python/Selenium to retrieve HTML files from ESPN.<br />
  Using selenium in order to get the HTML after javascript manipulation.<br />
  &nbsp;&nbsp; - NOT CURRENTLY PUSHING PYTHON CODE... additionally could use C# selenium.<br />

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
  


