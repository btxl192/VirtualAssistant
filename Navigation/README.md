# Navigation

## Client Setup

Running the run.bat file will open a gui that will prompt the user to upload floor plan. If it has already been run and want to update already configures floors, the folder corresponding to the floor been updated should be deleted first as well as the Blue.json file if the assistant is on the updated floor.

### IMPORTANT
T	he Navigation folder and the ".exe" unity file should follow either one of the following structures:

	Project                                    Project
	|                                          |
	|-Navigation                               |-Navigation
	|                                          |
	|-Avatar                                   |-Avatar
		|                                          |
		|-Avatar.exe                               |-BuildFolder
		                                                       |
		                                                       |-Avatar.exe

### IMPORTANT:
	Make sure that the only black (grey) pixels in the plans you upload are where the walls are (no black letters, signs, etc).

Then the script will detect where the rooms are and prompt the user to enter their names. If the room is hall or lift the user should not enter a name but press the corresponding buttons. It assumes that there exactly one hall and one lift on each floor (both buttons should be pressed exactly once per floor).

### IMPORTANT
	It should be possible to reach each room from the hall.

Then the script will ask the user if the assistant is on this floor and if yes, the user should click where it is on the picture of the floor.

### IMPORTANT
	The assistatn should be somewhere in the hall.

Then it will ask the user to either upload another plan or exit the GUI. The location of the assistant is set only once and after it is set it will not ask the user for it again. After the GUI is closed the script will produce videos with images that show how to get to each room. If the room is on the floor where the assistant is, it shows the route from the assistant to the desired room, and if the room is not on the same floor as the assistant it will show you a route from the assistant to the lift and from the lift to the desired room.

After the bat file is finished running, all the videos should be created and the navigation is ready to be used.

## Server Setup

On the server, you should create a "navigation.json" file in the API-Code directory, that contains all the rooms with text navigations for how to get to them. These navigations are what the assistant will say. The file should have the following syntax:
```json
{
	"1": {
		"roomA" : "how to get to roomA",
		"roomB" : "how to get to roomB"
	},
	"2": {
		"roomC" : "how to get to roomC",
		"roomD" : "how to get to roomD"
	}
}
```
where 1 and 2 are the floor numbers (in quotes), "roomA-D" are room names and "how to get to roomA-D" are directions to the corresponding room.
