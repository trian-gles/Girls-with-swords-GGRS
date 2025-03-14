extends Label

var last_inputs: int = 0

var displayed_inps = 0
var max_displayed_inps = 9

func clear():
	displayed_inps = 0
	text = ''

func display_input(input: String):
	text = input + '\n' + text
	displayed_inps += 1
	
	while (displayed_inps > max_displayed_inps):
		var i = len(text) - 1
		while (text[i] != '\n'):
			i -= 1
		text = text.substr(0, i)
		displayed_inps -= 1

func inputs(inps):
	## UP
	if (inps & 1) and not(last_inputs & 1):
		if (inps & 4):
			display_input('9')
		elif(inps & 8) :
			display_input('7')
		else:
			display_input('8')
			
	if !(inps & 1) and(last_inputs & 1) :
		if (inps & 4):
			display_input('6')
		elif(inps & 8) :
			display_input('4')
			
	# DOWN
	
	if (inps & 2) and not(last_inputs & 2):
		if (inps & 4):
			display_input('3')
		elif(inps & 8) :
			display_input('1')
		else:
			display_input("2")
			
	if !(inps & 2) and(last_inputs & 2) :
		if (inps & 4):
			display_input('6')
		elif(inps & 8) :
			display_input('4')
	
	# RIGHT
	
	if (inps & 4) and not(last_inputs & 4):
		if (inps & 1):
			display_input('9')
		elif(inps & 2) :
			display_input('3')
		else:
			display_input('6')
			
	if !(inps & 4) and(last_inputs & 4) :
		if (inps & 1):
			display_input('8')
		elif(inps & 2) :
			display_input("2")
			
	# LEFT
			
	if (inps & 8) and not(last_inputs & 8):
		if (inps & 1):
			display_input('7')
		elif(inps & 2) :
			display_input('1')
		else:
			display_input('4')
			
	if !(inps & 8) and(last_inputs & 8) :
		if (inps & 1):
			display_input('8')
		elif(inps & 2) :
			display_input("2")
			
	# BUTTONS
			
	if (inps & 16) and not(last_inputs & 16):
		display_input('p')
		
	if (inps & 32) and not(last_inputs & 32):
		display_input('k')
		
	if (inps & 64) and not(last_inputs & 64):
		display_input('s')
		
	if (inps & 128) and not(last_inputs & 128):
		display_input('a')
		
	if (inps & 256) and not(last_inputs & 256):
		display_input('b')
		
	if (inps & 512) and not(last_inputs & 512):
		display_input('c')


	last_inputs = inps
