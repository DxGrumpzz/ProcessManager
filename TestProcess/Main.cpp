#include <iostream>


// Simple executable that takes some arguments. 
// Used as a test for the ProcessManager
int main(int argc, char* argv[])
{
    if (argc == 1)
    {
        std::cout << "Running with default argument" << "\n" << argv[0] << "\n";
    }
    else
    {
        std::cout << "Running with "<< argc << " arguments:" << "\n";


        for (int a = 0; a < argc; a++)
        {
            std::cout << a << ". " << argv[a] << "\n";
        };
    };

    std::cin.get();

};