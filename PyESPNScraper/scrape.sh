#!/bin/bash
echo "Started scraping my team..."
python3 ESPNScraper.py
echo "Started scraping Dad's team..."
python3 ESPNScraper.py Dad 2
echo "Started scraping Miles's team..."
python3 ESPNScraper.py Miles 7

cd ./../
echo "Building my active stats..."
dotnet run
echo "Building Dad's active stats..."
dotnet run ./HTMLFiles/Dad/
echo "Building Miles's active stats..."
dotnet run ./HTMLFiles/Miles/
echo "Job finished."