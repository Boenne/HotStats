# HotStats
A desktop application to show statistics from Heroes of the Storm by analyzing the replay files

# Data
This program analyzes the replay files created by Heroes of the Storm located at:

../{user}/Documents/Heroes of the Storm/Accounts/{somerandomfolder}/{somerandomfolder}/Replays

It uses barrett777's awesome Heroes.ReplayParser: https://github.com/barrett777/Heroes.ReplayParser, to parse each replay file

# About
![alt tag](https://github.com/Boenne/HotStats/blob/master/src/HotStats/Resources/Readme/Program.png)

The program shows you an overview of all games played, average stats for all games played, wins and losses, and total stats.
You can also click a match in the 'Matches' view to see individual games.
It also shows how many times you've played with and against other heroes, and the percentage of wins and losses with and against those heroes. 

The highlighted section of the image below shows the following:
- I've played against Valla 255 times. I've won 53,7% and lost 46,3% of the games played against Valla.
- I've played with Valla 203 times. I've won 48,3% and lost 51,7% of the games played with Valla.
![alt tag](https://github.com/Boenne/HotStats/blob/master/src/HotStats/Resources/Readme/Heroes.png)

You can select a hero or add certain filters to get more specific stats.

#Layout
The program isn't tested on resolutions below 1920x1080, but i don't think it will look good below that resolution.
You can customize the following (click the cogwheel in the upper right corner):
- Background color
- Text color
- Border color
- Select a folder containing images to have a slideshow of background images instead of just a background color
