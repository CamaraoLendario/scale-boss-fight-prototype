shader_type particles;

uniform int particlesAmmount = 300;
uniform float scale = 10.;
uniform vec3[21] BonePoss;
uniform vec3[21] BoneDirs;
uniform float[21] BoneLens;
uniform float lenTotal;

vec3 rotateX(vec3 vector, float angle) {
	float cosine = cos(angle);
	float sine = sin(angle);
	
	return vec3(
		vector.x,
		cosine * vector.y - sine * vector.z,
		sine * vector.y + cosine * vector.z
	);
}
vec3 rotateY(vec3 vector, float angle) {
	float cosine = cos(angle);
	float sine = sin(angle);
	return vec3(
		cosine * vector.x + sine * vector.z,
		vector.y,
		-sine * vector.x + cosine * vector.z
	);
}
vec3 rotateZ(vec3 vector, float angle) {
	float cosine = cos(angle);
	float sine = sin(angle);
	return vec3(
		cosine * vector.x - sine * vector.y,
		sine * vector.x + cosine * vector.y,
		vector.z
    );
}
vec3 rotateVec3(vec3 vector, float x, float y, float z) {
	rotateX(vector, x);
	rotateY(vector, y);
	rotateZ(vector, z);
	return vector;
}
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
vec3 RotateToBoneDir(vec3 vec, vec3 boneDir){
	vec3 nBoneDir = normalize(boneDir * vec3(1., 1., -1.));
	vec = rotateZ(vec, atan(nBoneDir.y, sqrt(nBoneDir.x*nBoneDir.x + nBoneDir.z*nBoneDir.z)));
	vec = rotateY(vec, atan(nBoneDir.z, nBoneDir.x));
	return vec;
}

vec3 GetDebugCilinder(uint idx){
	return vec3(
		float(idx)*1.,
		vec2
		(sin(float(idx)),
		cos(float(idx)))*2.5
	);
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
	float randF = rand_from_seed(alt_seed);
	float fIdx = float(INDEX) / float(particlesAmmount);

	fIdx *= lenTotal;
	int index = int(fIdx*21.) % 21;
	float currentLen = 0.;
	for(int i = 0; i < int(INDEX) ; i++){
		currentLen += BoneLens[i];
		if (currentLen > fIdx){
			index = i;
			break;
		}
	}
	
	//int isUneven = index % 2;
	vec3 BonePos = BonePoss[index];
	vec3 BoneDir = BoneDirs[index];
	float boneLen = BoneLens[index];
	//TRANSFORM[3].xyz = BonePos + (BoneDir * ((sin(TIME*5.)*0.5) + .5));
	//TRANSFORM[3].xyz *= 10.;
	
	//float D =
	//BoneDir.x * BonePos.x +
	//BoneDir.y * BonePos.y +
	//BoneDir.z * BonePos.z;
	vec3 pos = vec3(
		randF,
		normalize(vec2(
		cos(PI * 2. * fract(randF * 587.)),
		sin(PI * 2. * fract(randF * 157.))))*0.1
	);
	pos *= boneLen;
	//pos = vec3((sin(TIME)+1.)/2., 0., 0.);
	
	pos = RotateToBoneDir(pos, BoneDir);
	
	TRANSFORM[3].xyz = (BonePos + pos) * scale;
	//TRANSFORM[3].xyz = (BonePos + BoneDir * (sin(TIME*3.)*.5 + .5))*10.;
	//TRANSFORM[3].xyz = GetDebugCilinder(INDEX);
}
