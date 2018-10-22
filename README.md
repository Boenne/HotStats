# HotStats
A desktop application to show statistics from Heroes of the Storm by analyzing the replay files

# Data
This program analyzes the replay files created by Heroes of the Storm located at:

../{user}/Documents/Heroes of the Storm/Accounts/{somerandomfolder}/{somerandomfolder}/Replays

It uses a forked version of barrett777's awesome Heroes.ReplayParser to parse each replay file.
- Original: https://github.com/barrett777/Heroes.ReplayParser 
- Fork: https://github.com/Boenne/Heroes.ReplayParser

# About
![alt tag](https://github.com/Boenne/HotStats/blob/master/Readme%20images/Program.png)

The program shows you an overview of all games played, the heroes you've played, average stats for all games played, wins and losses, and total stats.
You can also click a match in the 'Matches' view to see individual games.
It also shows how many times you've played with and against other heroes, and the percentage of wins and losses with and against those heroes. 

The highlighted section of the image below shows the following:
- I've played 303 games with Valla on the enemy team and of those I've won 53,1% and lost 46,9%.
- I've played 234 games with Valla on my team and of those I've won 47,0% and lost 53,0%.
![alt tag](https://github.com/Boenne/HotStats/blob/master/Readme%20images/Heroes.png)

# Filtering
You can select a hero that you've played and/or add other filters to get more specific stats.

# Layout
You can customize the following (click the cogwheel in the upper right corner):
- Background color
- Text color
- Border color
- Specify a path to an image to use as background or even a folder containing images. If you specify a folder path the program will change the background every 10 seconds
- Specify whether the program should use mastery portraits or normal portraits


The program isn't tested on resolutions below 1920x1080, but i'm sure it'll be bad.

# Download
The newest version of the program can be downloaded here: https://github.com/Boenne/HotStats/raw/master/Program/Program.zip
