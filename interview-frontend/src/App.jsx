import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import Layout from "./components/Layout/Layout";
import { startPlan } from "./api/api";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const App = () => {
    const navigate = useNavigate();
    const [inputVisible, setInputVisible] = useState(false);
    const [inputValue, setInputValue] = useState("");

    // Function to handle the start button click
    const start = async () => {
        if (!inputVisible) {
            setInputVisible(true);
            return;
        }

        if (inputValue.trim() === "") {
            toast.error("Please enter a value before proceeding.");
            return;
        }

        try {
            await startPlan(inputValue); // Assuming startPlan is a function that interacts with the backend
            navigate(`/plan/${inputValue}`);
        } catch (error) {
            toast.error("Failed to start plan. Please try again.");
        }
    };

    // Function to generate a random number
    const generateRandomNumber = () => {
        const randomNumber = Math.floor(Math.random() * 1000000); // Generates a random number (0–999999)
        setInputValue(randomNumber.toString());
    };

    // Function to handle input change (only allow numbers)
    const handleInputChange = (e) => {
        const value = e.target.value;
        if (/^\d*$/.test(value)) { // Regex to allow only numbers
            setInputValue(value);
        }
    };

    return (
        <Layout>
            <div className="container d-flex justify-content-center align-items-center vh-100">
                <div className="text-center w-100" style={{ maxWidth: "400px" }}>
                    <h3 className="mb-4">Start Here</h3>
                    <p className="mb-3">Click "Start" to begin</p>
                    {inputVisible && (
                        <div className="mb-3">
                            <input
                                type="text"
                                className="form-control mb-2"
                                placeholder="Enter your plan ID (numbers only)"
                                value={inputValue}
                                onChange={handleInputChange}
                            />
                            <button
                                className="btn btn-secondary w-100"
                                onClick={generateRandomNumber}
                            >
                                Generate Random Number
                            </button>
                        </div>
                    )}
                    <button
                        className="btn btn-primary w-100"
                        onClick={start}
                    >
                        {inputVisible ? "Submit" : "Start"}
                    </button>
                </div>
            </div>
            {/* Toast container */}
            <ToastContainer />
        </Layout>
    );
};

export default App;
