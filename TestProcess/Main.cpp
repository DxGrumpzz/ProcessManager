#include <string>
#include <fstream>
#include <thread>
#include <iostream>
#include <algorithm>
#include <filesystem>

/* Check if a file exists
@param name The file to check
*/
bool FileExists(const std::string& name)
{
    bool result = std::filesystem::exists(name);

    return result;
}

char asciitolower(const char& in)
{
    if (in <= 'Z' && in >= 'A')
        return in - ('Z' - 'z');
    return in;
}

// A "simple" process that takes a file as an argument and continuously re-creates it if it's deleted or renamed

int main(int argc, char* argv[])
{
    // Check if file path arg exists
    if (argc < 2)
    {
        std::cout << "Error: Missing file arg";
        std::cin.get();
        return 1;
    };

    // Attach a debugger if requested
    if (argc > 2)
    {
        // "normalize" the string to lower case
        std::string s(argv[2]);
        std::transform(s.begin(), s.end(), s.begin(), asciitolower);

        if (s == "true")
        {
            std::cout << "Attach debugger and press enter to continue\n";
            std::cin.get();
        };
    };

    // Setup file path
    std::string filePath(argv[1]);


    int counter = 0;
    while (1)
    {
        std::this_thread::sleep_for(std::chrono::seconds(1));

        // Check if file exists
        if (FileExists(filePath))
        {
            std::cout << counter << " File exist \n";
        }
        // If file was removed or renamed
        else
        {
            std::cout << counter << " Creating file...\n";

            // Create file 
            std::ofstream file;

            file.open(filePath);

            if (file.fail())
            {
                const size_t size = 256;
                char errmsg[size];

                strerror_s(errmsg, size, errno);
                
                std::cout << "Error: " << errmsg << "\n";
                std::cout << "File: " << filePath << "\n";

                std::cin.get();
                return 1;
            };
            
            file.close();
        };

        counter++;
    };
};