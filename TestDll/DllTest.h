#pragma once

#define  TEST_API  __declspec(dllexport)

struct TEST_DATA
{
	int size;
	char* value;
};


extern "C"
{
	TEST_API double Add(double a, double b);

	TEST_API int TestStruct(TEST_DATA dtin, TEST_DATA* outdata);
}