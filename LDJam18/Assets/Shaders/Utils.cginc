#define PI				3.1415926535897932384626433832795
#define TWO_PI			6.283185307179586476925286766559 
#define DIVIDE_PI		0.31830988618379067153776752674503
#define DIVIDE_TWO_PI	0.15915494309189533576888376337251
#define EPSILON			0.0000000000000000000000000000000000001

float remap(float fromMin, float fromMax, float toMin, float toMax, float value) {
	return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
}
inline float ease_in_quad(const float t)
{
    return t * t;
}

inline float ease_out_quad(const float t)
{
    float t_minus_one = t - 1;
    return 1 - (t_minus_one * t_minus_one);
}
inline float ease_out_circ(const float t)
{
    float one_minus_t = t - 1;
    return sqrt(1 - one_minus_t * one_minus_t);
}

inline float round_quad(float t)
{
    t = t * 2 - 1;
    t = -t * t + 1.0;
    return t;
}

float circ(float len, float progress, float width)
{
    float output = len;
    output = output * (1 / width) - ((progress - (width)) / width);
    output = round_quad(output);
    output = clamp(output, 0., 1.);
    return output;
}