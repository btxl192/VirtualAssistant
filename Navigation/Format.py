import numpy as np
from PIL import Image

class Format:

	def __init__(self, image, name):
		self.name = name
		self.image = image
		im = Image.open(image)
		self.arr = np.array(im)

	# def isBlack(self, x, y):
	# 	if x >= len(self.arr[0]):
	# 		return True
	# 	if self.arr[y][x][0] != self.arr[y][x][1]:
	# 		return False
	# 	if self.arr[y][x][1] != self.arr[y][x][2]:
	# 		return False
	# 	if self.arr[y][x][0] != self.arr[y][x][2]:
	# 		return False
	# 	if self.arr[y][x][2] > 220:
	# 		return False
	# 	return True

	def resize(self, img):
		size = 1280, 720
		return img.resize(size, Image.ANTIALIAS)

	def isBlack(self, x, y):
		if x >= len(self.arr[0]):
			return True
		if self.arr[y][x][0] < 90 and self.arr[y][x][1] < 90 and self.arr[y][x][2] < 90:
			return True
		return False

	def fixBlacks(self):
		for y in range(len(self.arr)):
			for x in range(len(self.arr[0])):
				if self.isBlack(x, y-1) and self.isBlack(x, y+1) and self.isBlack(x-1, y) and self.isBlack(x+1, y):
					for i in range(3):
						self.arr[y][x][i] = 0

	def removeColors(self):
		for y in range(len(self.arr)):
			for x in range(len(self.arr[0])):
				if not self.isBlack(x, y):
					for i in range(3):
						self.arr[y][x][i] = 255
				else:
					for i in range(3):
						self.arr[y][x][i] = 0


	def makeNewPicture(self):
		self.removeColors()
		img = Image.fromarray(self.arr)
		img = self.resize(img)
		self.arr = np.array(img)
		self.fixBlacks()	
		img = Image.fromarray(self.arr)
		img.save(self.name)


