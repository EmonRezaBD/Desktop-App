//#include <string>
//#include <iostream>
//#include <zmq.hpp>
//
//int main()
//{
//    // initialize the zmq context with a single IO thread
//    zmq::context_t context{ 1 };
//    // construct a REQ (request) socket and connect to interface
//    zmq::socket_t socket{ context, zmq::socket_type::req };
//    socket.connect("tcp://localhost:5555");
//
//    // set up some static data to send
//    const std::string data{ "Hello" };
//
//    for (auto request_num = 0; request_num < 10; ++request_num)
//    {
//        // send the request message
//        std::cout << "Sending Hello " << request_num << "..." << std::endl;
//
//        // Create a message from the data
//        zmq::message_t message(data.size());
//        memcpy(message.data(), data.data(), data.size());
//        socket.send(message); // Removed flags
//
//        // wait for reply from server
//        zmq::message_t reply{};
//        socket.recv(&reply); // Changed to pointer
//
//        std::string reply_str(static_cast<char*>(reply.data()), reply.size());
//        std::cout << "Received " << reply_str;
//        std::cout << " (" << request_num << ")";
//        std::cout << std::endl;
//    }
//    return 0;
//}



#include <string>
#include <iostream>
#include <zmq.hpp>
#include <nlohmann/json.hpp>  // for JSON parsing

using json = nlohmann::json;

// Define the Person structure to match C#
struct Person {
    std::string name;
    int age;
};

// JSON deserialization
void from_json(const json& j, Person& p) {
    j.at("Name").get_to(p.name);
    j.at("Age").get_to(p.age);
}

int main() {
    try {
        // Initialize the ZMQ context
        zmq::context_t context(1);
        zmq::socket_t socket(context, zmq::socket_type::rep);
        socket.bind("tcp://*:5555");

        std::cout << "Server started, waiting for messages..." << std::endl;

        while (true) {
            zmq::message_t request;

            // Receive the message
            socket.recv(&request);

            // Convert message to string
            std::string json_str(static_cast<char*>(request.data()), request.size());
            std::cout << "Received message: " << json_str << std::endl;

            try {
                // Parse JSON and convert to Person object
                auto j = json::parse(json_str);
                Person person = j.get<Person>();

                // Print the received data
                std::cout << "Received Person Details:" << std::endl;
                std::cout << "Name: " << person.name << std::endl;
                std::cout << "Age: " << person.age << std::endl;

                // Send a reply (required for REQ/REP pattern)
                std::string reply = "Message received successfully";
                zmq::message_t reply_msg(reply.size());
                memcpy(reply_msg.data(), reply.data(), reply.size());
                socket.send(reply_msg);
            }
            catch (const json::exception& e) {
                std::cerr << "JSON parsing error: " << e.what() << std::endl;

                // Send error reply
                std::string error_reply = "Error processing message";
                zmq::message_t reply_msg(error_reply.size());
                memcpy(reply_msg.data(), error_reply.data(), error_reply.size());
                socket.send(reply_msg);
            }
        }
    }
    catch (const zmq::error_t& e) {
        std::cerr << "ZMQ error: " << e.what() << std::endl;
        return 1;
    }

    return 0;
}