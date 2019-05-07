#ESPNScraper.py --- COULD PROBABLY ACTUALLY DO THIS IN C# (w/selenium)
from selenium import webdriver
import time
import os, sys
from datetime import date

browser = webdriver.Chrome('./chromedriver') #replace with .Firefox(), or with the browser of your choice

htmlFileDir = './../HTMLFiles/'
# end day, team id, and league id
currentDay = (date.today() - date(2019, 3, 20)).days;
teamId = 9;    # dadTeamId = 2;
leagueId = 8505320;

# Handle command line arguments: ESPNScraper.py [html file extension] [teamId]
# ex... ESPNScraper.py Dad 2
if (len(sys.argv) > 1):
   htmlFileDir += sys.argv[1] + "/"
   if (len(sys.argv) > 2):
      teamId = int(sys.argv[2]);

# Gets number of days already scraped from html directory 
# gets files in base directory then breaks out of directory
for dirName, subdirList, fileList in os.walk(htmlFileDir):
   lastdayupdated = len(fileList)
   break

# to switch teams change html file dir (destination of files) and teamId
for i in range(lastdayupdated + 1, currentDay + 1):
   file = htmlFileDir + "day" + str(i) + "HTML.html"
   f = open(file, "w+")

   url = ("http://fantasy.espn.com/baseball/team?leagueId=" + str(leagueId) 
         + "&teamId=" + str(teamId) 
         + "&scoringPeriodId=" + str(i) 
         + "&statSplit=singleScoringPeriod")

   print(url)
   browser.get(url) #navigate to the page

   time.sleep(3)
   innerHTML = browser.execute_script("return document.body.innerHTML") #returns the inner HTML as a string

   f.write(innerHTML)
   f.close()