#include "stdafx.h"

#include "DllTest.h"



double Add(double a, double b)
{
	return a + b;

}

int TestStruct(TEST_DATA indt, TEST_DATA* outdata)
{
	outdata->size = 10;
	outdata->value = (char*)TEXT("helloworld");

	return 0;
	
}
