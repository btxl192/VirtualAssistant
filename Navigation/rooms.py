import numpy as np
from PIL import Image

class Pixel:

	def __init__(self, x, y, color):
		self.colors = color.copy()
		self.x = x
		self.y = y
		self.room = None

class Room:

	def __init__(self, x, x1, y):
		self.pixelss = {y : [(x, x1)]}
		self.useless = False
		self.roomsToAdd = []

	def add(self, x, x1, y):
		if y in self.pixelss:
			dummy = self.pixelss[y]
			dummy.append((x, x1))
			self.pixelss.update({y : dummy})
		else:
			self.pixelss[y] = [(x, x1)]

	def addRoom(self, a):
		for k in a.pixelss:
			if k in self.pixelss:
				dummy = self.pixelss[k]
				for i in range(len(a.pixelss[k])):
					if a.pixelss[k][i] not in dummy:
						dummy.append(a.pixelss[k][i])
				self.pixelss.update({k : dummy})
			else:
				self.pixelss[k] = a.pixelss[k]
		a.useless = True

class Work:

	def __init__(self, file):
		image = Image.open(file)
		self.im = np.array(image)
		self.rooms = []
		self.pixels = []
		for y in range(len(self.im)):
			for x in range(len(self.im[0])):
				self.pixels.append(Pixel(x, y, self.im[y][x]))


	# def isBlack(self, x, y):
	# 	if x >= len(self.im[0]):
	# 		return True
	# 	if self.im[y][x][0] < 90 and self.im[y][x][1] < 90 and self.im[y][x][2] < 90:
	# 		return True
	# 	return False

	def isBlack(self, x, y):
		if x >= len(self.im[0]):
			return True
		if self.im[y][x][0] != self.im[y][x][1]:
			return False
		if self.im[y][x][1] != self.im[y][x][2]:
			return False
		if self.im[y][x][0] != self.im[y][x][2]:
			return False
		if self.im[y][x][2] > 220:
			return False
		return True

	def checkRange(self, range1, range2):
		x = range1[0]
		y = range1[1]
		c = range2[0]
		d = range2[1]
		if x <= c and y <= d and y >= c:
			return True
		if x >= c and x <= d and y >= d:
			return True
		if x >= c and x <= d and y <= d and y >= c:
			return True
		if x <= c and y >= d:
			return True
		return False

	def nextRooms(self, a, b):
		for i in a.pixelss:
			if (i+1) in b.pixelss:
				for m in a.pixelss[i]:
					for n in b.pixelss[i+1]:
						if self.checkRange(m, n):
							return True
			if (i-1) in b.pixelss:
				for m in a.pixelss[i]:
					for n in b.pixelss[i-1]:
						if self.checkRange(m, n):
							return True
		return False

	def work(self):
		for y in range(len(self.im)):
			x = 0
			while x < len(self.im[0]):
				if self.isBlack(x, y):
					x += 1
					continue
				x1 = x
				while not self.isBlack(x1+1, y):
					x1 += 1
				a = self.getRoom(x, x1, y)
				if a is not None:
					a.add(x, x1, y)
					for k in range(x, x1+1):
						self.pixels[len(self.im[0]) * y + k].room = a
				else:
					a = Room(x, x1, y)
					for k in range(x, x1+1):
						self.pixels[len(self.im[0]) * y + k].room = a
					self.rooms.append(a)
				x = x1+1
				if x == len(self.im[0]) - 1:
					break

	def fixRooms(self):
		check = False
		for i in range(len(self.rooms) - 1):
			for j in range(i + 1, len(self.rooms)):
				if(self.nextRooms(self.rooms[i], self.rooms[j])):
					check = True
					self.rooms[i].roomsToAdd.append(j)
		for i in range(len(self.rooms)):
			addRooms = self.rooms[len(self.rooms) - 1 - i].roomsToAdd
			for j in addRooms:
				self.rooms[len(self.rooms) - 1 - i].addRoom(self.rooms[j])
			self.rooms[len(self.rooms) - 1 - i].roomsToAdd = []
		newRooms = []
		for i in range(len(self.rooms)):
			if not self.rooms[i].useless:
				newRooms.append(self.rooms[i])
		self.rooms = newRooms.copy()
		return check

	def fixFixRooms(self):
		self.work()
		check = True
		while check:
			check = self.fixRooms()
		self.rooms.pop(0)
		self.makePicture()

	def getNumRooms(self):
		return len(self.rooms)
			

	def getRoom(self, x, x1, y):
		if y == 0:
			return None
		for k in range(x, x1+1):
			if not self.isBlack(k, y-1):
				return self.pixels[len(self.im[0])* (y-1) + k].room
		return None

	def getRoomPixels(self, room):
		return self.rooms[room].pixelss

	def makePicture(self):
		for k in range(len(self.rooms)):
			newArr = self.im.copy()
			for y in self.rooms[k].pixelss:
				for x in self.rooms[k].pixelss[y]:
					for x1 in range(x[0], x[1]+1):
						for i in range(3):
							newArr[y][x1][i] = 0
			name = 'room' + str(k)
			img = Image.fromarray(newArr)
			img.save('floorRooms/' + name + '.jpg')
