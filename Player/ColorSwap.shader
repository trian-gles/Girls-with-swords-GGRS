shader_type canvas_item;

uniform int numberOfColors : hint_range(1, 16);
uniform vec4 C1 : hint_color;
uniform vec4 C2 : hint_color;
uniform vec4 C3 : hint_color;
uniform vec4 C4 : hint_color;
uniform vec4 C5 : hint_color;
uniform vec4 C6 : hint_color;
uniform vec4 C7 : hint_color;
uniform vec4 C8 : hint_color;
uniform vec4 C9 : hint_color;
uniform vec4 C10 : hint_color;
uniform vec4 C11 : hint_color;
uniform vec4 C12 : hint_color;
uniform vec4 C13 : hint_color;
uniform vec4 C14 : hint_color;
uniform vec4 C15 : hint_color;
uniform vec4 C16 : hint_color;

uniform int brightness = 0;

void fragment(){
	vec4 referenceColors[16];
	referenceColors[0] = C1;
	referenceColors[1] = C2;
	referenceColors[2] = C3;
	referenceColors[3] = C4;
	referenceColors[4] = C5;
	referenceColors[5] = C6;
	referenceColors[6] = C7;
	referenceColors[7] = C8;
	referenceColors[8] = C9;
	referenceColors[9] = C10;
	referenceColors[10] = C11;
	referenceColors[11] = C12;
	referenceColors[12] = C13;
	referenceColors[13] = C14;
	referenceColors[14] = C15;
	referenceColors[15] = C16;
	
	vec4 textureColor = texture(TEXTURE, UV);
	textureColor.rgb = referenceColors[int(floor(textureColor.r * float(numberOfColors)))].rgb;
	COLOR.a = textureColor.a;
	COLOR.rgb = mix(textureColor.rgb, vec3(1, 1, 1).rgb, 0);
}

