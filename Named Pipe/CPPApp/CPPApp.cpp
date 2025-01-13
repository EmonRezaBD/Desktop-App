#include <windows.h>
#include <iostream>
#include <string>
#include <nlohmann/json.hpp> // Include nlohmann/json library for JSON parsing
#include <chrono> // Include the chrono library
#include<time.h>


using json = nlohmann::json;

int main()
{
    HANDLE hPipe = CreateFile(TEXT("\\\\.\\pipe\\myNamedPipe1"), GENERIC_READ,
        0, NULL, OPEN_EXISTING, 0, NULL);

    if (hPipe == INVALID_HANDLE_VALUE) {
        std::cerr << "Failed to connect to pipe." << std::endl;
        return 1;
    }

    char buffer[8192];
    DWORD bytesRead;

    // Read data from the pipe 10 times
    for (int i = 0; i < 100; i++) {

       /* time_t t;
        srand((unsigned)time(&t));
        clock_t start_time, end_time;
        start_time = clock();*/

        auto start = std::chrono::high_resolution_clock::now();


        if (ReadFile(hPipe, buffer, sizeof(buffer) - 1, &bytesRead, NULL)) {
            buffer[bytesRead] = '\0'; // Null-terminate the string

            //end_time = clock();
            auto end = std::chrono::high_resolution_clock::now();
            std::chrono::duration<double> duration = end - start; // Calculate duration

            try {
                // Parse JSON data
                auto jsonData = json::parse(buffer);
                //std::cout << "Received data " << i + 1 << ":" << std::endl;
                std::cout << "Name: " << jsonData["Name"] << std::endl;
                std::cout << "Team Name: " << jsonData["TeamName"] << std::endl;
                std::cout << "Members: " << jsonData["Members"] << std::endl;
                // Print the vector of doubles
                std::cout << "Scores: ";
                for (const auto& score : jsonData["Scores"]) {
                    std::cout << score.get<double>() << " "; // Print each score
                }
                std::cout << std::endl << std::endl;

                // Output the time taken to receive the data
               /* printf("Total receiving time : %4.6f \n",
                    (double)((double)((end_time - start_time) / CLOCKS_PER_SEC)));*/
                    // Output the time taken to receive the data
                std::cout << "Time taken to receive data: " << duration.count() * 1000.0 << " ms" << std::endl; // Convert to milliseconds

            }
            catch (const json::parse_error& e) {
                std::cerr << "JSON parse error: " << e.what() << std::endl;
                std::cerr << "Received raw data: " << buffer << std::endl; // Log raw data for debugging
            }
        }
        else {
            std::cerr << "Failed to read from pipe." << std::endl;
        }
    }

    CloseHandle(hPipe);
    return 0;
}
