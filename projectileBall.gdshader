shader_type particles;

uniform float radius = 1.;
uint hash(uint x) {
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = (x >> uint(16)) ^ x;
	return x;
}
float rand_from_seed(inout uint seed) {
	int k;
	int s = int(seed);
	if (s == 0) {
		s = 305420679;
	}
	k = s / 127773;
	s = 16807 * (s - k * 127773) - 2836 * k;
	if (s < 0) {
		s += 2147483647;
	}
	seed = uint(s);
	return float(seed % uint(65536)) / 65535.0;
}
void start() {
	uint alt_seed = hash(INDEX + uint(1.) + RANDOM_SEED);
	vec3 angles = vec3(
		rand_from_seed(alt_seed) * PI * 2.,
		rand_from_seed(alt_seed) * PI * 2.,
		rand_from_seed(alt_seed) * PI * 2.
	);
	
	vec2[3] trigs = {
		vec2(cos(angles.x), sin(angles.x)),
		vec2(cos(angles.y), sin(angles.y)),
		vec2(cos(angles.z), sin(angles.z))
	};
	
	TRANSFORM[0].xyz = vec3(
		trigs[1].x * trigs[2].x,
		trigs[1].x * trigs[2].y,
		-trigs[1].y
	);
	TRANSFORM[1].xyz = vec3(
		trigs[0].y * trigs[1].y * trigs[2].x - trigs[0].x * trigs[2].y,
		trigs[0].y * trigs[1].y * trigs[2].y + trigs[0].x * trigs[2].x,
		trigs[0].y * trigs[1].x
	);
	TRANSFORM[2].xyz = vec3(
		trigs[0].x * trigs[1].y * trigs[2].x + trigs[0].y * trigs[2].y,
		trigs[0].x * trigs[1].y * trigs[2].y - trigs[0].y * trigs[2].x,
		trigs[0].x * trigs[1].x
	);
}

void process() {
	uint alt_seed = hash(INDEX + uint(1.) + RANDOM_SEED);
	float frand = rand_from_seed(alt_seed);
	vec3 pos = vec3(
		cos(float(INDEX)),
		sin(float(INDEX)),
		0.) * radius;
	
	vec3 mainPoint = normalize(vec3(1.)) * radius;
	float sphereModA = (pos.x - mainPoint.x) * (pos.x - mainPoint.x);
	float sphereModB = (pos.y - mainPoint.y) * (pos.y - mainPoint.y);
	float radiusSquared = radius * radius;
	//sphereModA + sphereModB + (z - mainPoint.z) * (z - mainPoint.z) = radiusSquared
	//(z - mainPoint.z) * (z - mainPoint.z) = radiusSquared - (sphereModA + sphereModB)
	float z = sqrt(radiusSquared - (sphereModA + sphereModB));
	if (frand < 0.5){
		z *= -1.;
	}
	z += mainPoint.z;
	TRANSFORM[3].xyz = vec3(
		cos(float(INDEX)),
		sin(float(INDEX)),
		z);
}
