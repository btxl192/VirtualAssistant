import tkinter as tk
from tkinter import filedialog
from PIL import ImageTk, Image  
import rooms
import Format
import json
import os.path
from os import path
import os


class GUI:

	def __init__(self):
		curDir = os.getcwd()
		path = os.path.join(curDir, "floorRooms")
		os.mkdir(path)
		self.window = tk.Tk()
		self.window.protocol("WM_DELETE_WINDOW", self.quit)
		self.window.title("Blue Navigation")
		self.window.geometry("1200x650")
		self.img = None
		self.mainLabelFrame = None
		self.questionFrame = None
		self.mainLabel = None
		self.questionLabel = None
		self.questionEntry = None
		self.questionButton = None
		self.questionButton1 = None
		self.questionButton2 = None
		self.questionButton3 = None
		self.filename = None
		self.numOfRooms = 0
		self.work = None
		self.currentRoom = 0
		self.floor = 0
		self.hasBlueBeenSet = False
		self.rooms = []

	def initializeWindow(self):
		self.mainLabelFrame = tk.Frame(self.window, height=10)
		self.mainLabelFrame.pack(side=tk.TOP)

		self.questionFrame = tk.Frame(self.window, height=5)
		self.questionFrame.pack(side=tk.BOTTOM)

		self.mainLabel = tk.Label(self.mainLabelFrame, height=10 ,text="Welcome to Blue Navigation",
							 font=("Courier", 25))
		self.mainLabel.pack()

		self.questionLabel = tk.Label(self.questionFrame, height=5, text="Upload a floor plan:", font=("Courier", 15))
		self.questionLabel.grid(column=0, row=0, padx=10)

		self.questionButton = tk.Button(self.questionFrame, text='Upload', command=self.uploadAction)
		self.questionButton.grid(column=2, row=0, padx=10)



	def uploadAction(self, event=None):
		self.filename = filedialog.askopenfilename()
		im = Image.open(self.filename)
		im = im.resize((960, 540), Image.ANTIALIAS)
		self.img = ImageTk.PhotoImage(im)
		self.mainLabel.destroy()
		self.mainLabel = tk.Label(self.mainLabelFrame, image = self.img)
		self.mainLabel.pack()
		self.questionLabel.config(text="Which floor is this: ")
		self.questionButton1 = tk.Button(self.questionFrame, text='Upload another', command = self.uploadAction)
		self.questionButton1.grid(column=3, row=0)
		self.questionButton.config(text='Submit', command = self.uploadFloor)
		self.questionEntry = tk.Entry(self.questionFrame)
		self.questionEntry.grid(column=1, row=0)
		

	def uploadFloor(self):
		num = self.questionEntry.get()
		flag = False
		try:
			num = int(num)
			if num >= 0:
				self.floor = num
				flag = True
			else:
				self.questionLabel.config(text='Please enter a vlid number, greater than 0: ')
		except ValueError:
			self.questionLabel.config(text='Please enter a vlid number, greater than 0: ')
		self.questionEntry.delete(0, tk.END)
		if flag:
			self.questionEntry.config(state='disabled')
			self.questionButton.config(state='disabled')
			self.mainLabel.destroy()
			self.mainLabel = tk.Label(self.mainLabelFrame, height=10 ,text="Image is being processed. Please wait!", font=("Courier", 25))
			self.mainLabel.pack()
			f = Format.Format(self.filename, "floor" + str(self.floor) + ".jpg")
			f.makeNewPicture()
			self.filename = "floor" + str(self.floor) + ".jpg"
			self.work = rooms.Work(self.filename)
			self.work.fixFixRooms()
			self.numOfRooms = self.work.getNumRooms()
			self.questionEntry.config(state='normal')
			self.questionButton.config(state='normal')
			im = Image.open('floorRooms/room0.jpg')
			im = im.resize((960, 540), Image.ANTIALIAS)
			self.img = ImageTk.PhotoImage(im)
			self.mainLabel.destroy()
			self.mainLabel = tk.Label(self.mainLabelFrame, image = self.img)
			self.mainLabel.pack()
			self.questionLabel.config(text='Which room is this: ')
			self.questionButton.config(command=self.uploadRoom)
			self.questionButton1.config(text='Not a Room', command=self.notRoom)
			self.questionButton2 = tk.Button(self.questionFrame, text='Lift', command = self.lift)
			self.questionButton2.grid(column=4, row=0, padx=10)
			self.questionButton3 = tk.Button(self.questionFrame, text='Hall', command = self.hall)
			self.questionButton3.grid(column=5, row=0)
			self.questionEntry.delete(0, tk.END)

	def notRoom(self):
		self.rooms.append(None)
		self.nextRoom()

	def lift(self):
		self.rooms.append("lift")
		self.questionButton2.config(state='disabled')
		self.nextRoom()

	def hall(self):
		self.rooms.append("hall")
		self.questionButton3.config(state='disabled')
		self.nextRoom()

	def uploadRoom(self):
		a = self.questionEntry.get()
		a = a.replace(" ", "")
		if a == "":
			return
		self.rooms.append(a)
		self.nextRoom()

	def deletePhotos(self):
		for i in range(self.numOfRooms):
			paths = os.path.join("floorRooms", "room" + str(i) + ".jpg")
			if path.exists(paths):
				os.remove(paths)
		
	def nextRoom(self):
		self.questionEntry.delete(0, tk.END)
		self.currentRoom = self.currentRoom + 1
		if self.currentRoom == self.numOfRooms:
			self.deletePhotos()
			im = Image.open(self.filename)
			im = im.resize((960, 540), Image.ANTIALIAS)
			self.img = ImageTk.PhotoImage(im)
			self.mainLabel.config(image = self.img)
			if "hall" not in self.rooms or "lift" not in self.rooms:
				self.questionLabel.config(text="Every floor should have a hall and a lift! ")
				self.questionButton.config(text="Upload another floor", command=self.uploadAction)
				self.questionButton1.config(text="Quit", command=self.quit)
				self.questionEntry.destroy()
				self.questionButton2.destroy()
				self.questionButton3.destroy()
				self.currentRoom = 0
				return
			self.writeToFile()
			if not self.hasBlueBeenSet:
				self.questionLabel.config(text="Is Blue on this floor? ")
				self.questionButton.config(text='Yes', command = self.enable_mouseposition)
				self.questionButton.grid(column=2, row=0, padx=10)
				self.questionEntry.destroy()
				self.questionButton2.destroy()
				self.questionButton3.destroy()
				self.questionButton1.config(text='No', command=self.uploadAnotherFloor)
				return
			else:
				self.questionButton2.destroy()
				self.questionButton3.destroy()
				self.questionLabel.config(text="Upload another plan: ")
				self.questionButton.config(text='Upload', command = self.uploadAction)
				self.questionButton.grid(column=2, row=0)
				self.questionButton1.config(text="Quit", command=self.quit)
				return
		path = 'floorRooms/room' + str(self.currentRoom) + '.jpg'
		im = Image.open(path)
		im = im.resize((960, 540), Image.ANTIALIAS)
		self.img = ImageTk.PhotoImage(im)
		self.mainLabel.destroy()
		self.mainLabel = tk.Label(self.mainLabelFrame, image = self.img)
		self.mainLabel.pack()

	def enable_mouseposition(self):
		self.questionButton.destroy()
		self.questionButton1.destroy()
		self.questionLabel.config(text="Please click where Blue is, on the picture!")
		self.mainLabel.bind("<Button-1>", self.get_mouseposition)
		self.window.config(cursor="crosshair")


	def get_mouseposition(self, event):
		self.hasBlueBeenSet = True
		if not path.exists("Blue.json"):
			self.createFile("Blue.json")
		dictBlue = {}
		dictCoor = {}
		y = round(event.y * 4 / 3)
		x = round(event.x * 4 / 3)
		dictCoor[y] = x
		dictBlue[self.floor] = dictCoor
		with open("Blue.json", "r+") as file:
			file.seek(0)
			json.dump(dictBlue, file, indent=4)
		self.mainLabel.unbind("<Button-1>")
		self.window.config(cursor="arrow")
		self.questionLabel.config(text="Upload another plan: ")
		self.questionButton = tk.Button(self.questionFrame, text='Upload', command = self.uploadAction)
		self.questionButton.grid(column=2, row=0)
		self.questionButton1 = tk.Button(self.questionFrame, text='Quit', command = self.quit)
		self.questionButton1.grid(column=3, row=0)

	def quit(self):
		self.deletePhotos()
		curDir = os.getcwd()
		path = os.path.join(curDir, "floorRooms")
		os.rmdir(path)
		self.window.destroy()

	def uploadAnotherFloor(self):
		self.questionLabel.config(text="Upload another plan: ")
		self.questionButton.config(text='Upload', command = self.uploadAction)
		self.questionButton.grid(column=2, row=0)
		self.questionButton1.config(text="Quit", command=self.quit)

	def createFile(self, name):
		f = open(name, "w")
		f.write("\t{\n\n\t}\n")
		f.close()

	def writeToFile(self):
		if not path.exists("floor" + str(self.floor) + ".json"):
			self.createFile("floor" + str(self.floor) + ".json")
		curDir = os.getcwd()
		p = os.path.join(curDir, str(self.floor))
		os.mkdir(p)
		dict_rooms = {}
		dict_floor = {}
		for i in range(self.numOfRooms):
			if self.rooms[i] is not None:
				dict_rooms[self.rooms[i]] = self.work.getRoomPixels(i)
		dict_floor[self.floor] =dict_rooms
		with open("floor" + str(self.floor) + ".json", "r+") as file:
			data = json.load(file)
			if data is None:
				data = dict_floor
			else:
				data.update(dict_floor)
			file.seek(0)
			json.dump(data, file, indent=4)
		self.rooms = []
		self.currentRoom = 0


	def run(self):
		self.initializeWindow()
		self.window.mainloop()

if __name__ == '__main__':
	GUI().run()
