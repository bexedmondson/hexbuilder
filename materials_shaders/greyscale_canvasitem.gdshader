shader_type canvas_item;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float c = mix(COLOR.r, COLOR.g, COLOR.b);
	COLOR.rgb = vec3(c);
}