#pragma once

using namespace System;

namespace WrapperDLL {
	public ref class WrapperClass
	{
		// TODO: Add your methods for this class here.
    public:
        int Add(int a, int b)
        {
            MathOperationsLibrary::MathOperations^ mathOps = gcnew MathOperationsLibrary::MathOperations();
            return mathOps->Add(a, b);
        }

        int Sub(int a, int b)
        {
            MathOperationsLibrary::MathOperations^ mathOps = gcnew MathOperationsLibrary::MathOperations();
            return mathOps->Subtract(a, b);
        }
	};
}
