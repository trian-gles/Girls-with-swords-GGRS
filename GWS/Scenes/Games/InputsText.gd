extends Label


var last_inputs: int = 0

var p1_last_inputs = 0

var displayed_inps = 0
var max_displayed_inps = 9

func clear():
	var displayed_inps = 0
	text = ''

func display_input(input):
	print("Displaying input " + input)
	text = input + '\n' + text
	displayed_inps += 1
	
	while (displayed_inps > max_displayed_inps):
		var i = len(text) - 1
		while (text[i] != '\n'):
			i -= 1
		text = text.substr(0, i)
		displayed_inps -= 1

func inputs(p1inps, p2inps):
	## UP
	if (p1inps & 1) and not (p1_last_inputs & 1):
		if (p1inps & 4):
			display_input('9')
		elif (p1inps & 8):
			display_input('7')
		else:
			display_input('í±»í ¾í±»í ¾í±»8')
			
	if !(p1inps & 1) and (p1_last_inputs & 1):
		if (p1inps & 4):
			display_input('6')
		elif (p1inps & 8):
			display_input('4')
			
	# DOWN
	
	if (p1inps & 2) and not (p1_last_inputs & 2):
		if (p1inps & 4):
			display_input('í±»í ¾í±»í ¾í±»3')
		elif (p1inps & 8):
			display_input('í±»í ¾í±»í ¾í±»1')
		else:
			display_input('í±»í ¾í±»í ¾í±»2')
			
	if !(p1inps & 2) and (p1_last_inputs & 2):
		if (p1inps & 4):
			display_input('6')
		elif (p1inps & 8):
			display_input('4')
	
	# RIGHT
	
	if (p1inps & 4) and not (p1_last_inputs & 4):
		if (p1inps & 1):
			display_input('9')
		elif (p1inps & 2):
			display_input('3')
		else:
			display_input('6')
			
	if !(p1inps & 4) and (p1_last_inputs & 4):
		if (p1inps & 1):
			display_input('8')
		elif (p1inps & 2):
			display_input('2')
			
	# LEFT
			
	if (p1inps & 8) and not (p1_last_inputs & 8):
		if (p1inps & 1):
			display_input('7')
		elif (p1inps & 2):
			display_input('1')
		else:
			display_input('í±»í ¾í±»í ¾í±»4')
			
	if !(p1inps & 8) and (p1_last_inputs & 8):
		if (p1inps & 1):
			display_input('8')
		elif (p1inps & 2):
			display_input('2')
			
	# BUTTONS
			
	if (p1inps & 16) and not (p1_last_inputs & 16):
		display_input('p')
		
	if (p1inps & 32) and not (p1_last_inputs & 32):
		display_input('k')
		
	if (p1inps & 64) and not (p1_last_inputs & 64):
		display_input('s')
			
	p1_last_inputs = p1inps
