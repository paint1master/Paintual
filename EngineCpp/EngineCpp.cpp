// EngineCpp.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <amp.h>
using namespace concurrency;

extern "C"             //No name mangling
__declspec(dllexport)  //Tells the compiler to export the function
int                    //Function return type     
__cdecl                //Specifies calling convention, cdelc is default, 
					   //so this can be omitted 
	testIncrement(int number) {
	return number + 1;
}

extern "C" __declspec(dllexport) double __cdecl 
	testMult(double a, double b) {
	return a * b;
}

extern "C" __declspec(dllexport) double __cdecl
testMultArray(double d[]) {
	double result = d[0] * d[1];
	return result;
}

extern "C" __declspec(dllexport) void __cdecl
angle(int n, double* divSpread255_array, double* divSpread2f_array, double* angle_array, double* lum_array) {
	array_view<double, 1> av_divSpread255(n, divSpread255_array);
	array_view<double, 1> av_divSpread2f(n, divSpread2f_array);
	array_view<double, 1> av_angle(n, angle_array);
	array_view<double, 1> av_lum(n, lum_array);

	parallel_for_each(av_angle.extent, [=](index<1> idx) restrict(amp)
	{
		av_angle[idx] = av_lum[idx] * av_divSpread255[idx] - av_divSpread2f[idx];
	});
}

extern "C" __declspec(dllexport) void __cdecl
luminance(int n, int* red, int* green, int* blue, double* lum)
{
	array_view<int, 1> av_red(n, red);
	array_view<int, 1> av_green(n, green);
	array_view<int, 1> av_blue(n, blue);
	array_view<double, 1> av_lum(n, lum);

	parallel_for_each(av_lum.extent, [=](index<1> idx) restrict(amp)
	{
		av_lum[idx] = (0.2126 * av_red[idx]) + (0.7152 * av_green[idx]) + (0.0722 * av_blue[idx]);
	});
}

extern "C" __declspec(dllexport) void __cdecl
calculateRippleEffect(int n, double* sum, double* b1_1, double* b1_2, double* b1_3, double* b1_4, double* b2_1, double t_dampening)
{
	array_view<double, 1> av_sum(n, sum);
	array_view<double, 1> av_b1_1(n, b1_1);
	array_view<double, 1> av_b1_2(n, b1_2);
	array_view<double, 1> av_b1_3(n, b1_3);
	array_view<double, 1> av_b1_4(n, b1_4);
	array_view<double, 1> av_b2_1(n, b2_1);

	parallel_for_each(av_sum.extent, [=](index<1> idx) restrict(amp)
	{
		av_sum[idx] = av_b1_1[idx] + av_b1_2[idx] + av_b1_3[idx] + av_b1_4[idx];
		av_sum[idx] = av_sum[idx] / 4;
		av_sum[idx] -= av_b2_1[idx];
		av_sum[idx] = av_sum[idx] * t_dampening;
	});

}



