import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { startPlan } from "./api/api";
import Layout from "./components/Layout/Layout";
import { ThreeDots } from "react-loader-spinner";

const App = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const start = async () => {
        setLoading(true);
        try {
            const plan = await startPlan();
            navigate(`/plan/${plan.planId}`);
        } catch (error) {
            console.error("Failed to start plan:", error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <Layout>
            <div className="container">
                <div className="text-center mt-4">
                    <h3>Start Here</h3>
                    <p>Click "start" to begin</p>
                    <button className="btn btn-primary" onClick={start} disabled={loading}>
                        Start
                    </button>
                    {loading && (
                        <ThreeDots
                            height="80"
                            width="80"
                            radius="9"
                            color="#00BFFF"
                            ariaLabel="three-dots-loading"
                            visible={true}
                        />
                    )}
                </div>
            </div>
        </Layout>
    );
};

export default App;