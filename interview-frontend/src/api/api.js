const api_url = "https://localhost:10011";

export const startPlan = async (planId) => {
    const url = `${api_url}/Plan`;

    const requestBody = {
        planId,
        createDate: new Date().toISOString(),
        updateDate: new Date().toISOString(),
        planProcedures: []
    };

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                Accept: "application/json",
                "Content-Type": "application/json",
            },
            body: JSON.stringify(requestBody),
        });

        if (!response.ok) {
            const errorMessage = await response.text(); // Get error details
            throw new Error(`Failed to create plan: ${errorMessage}`);
        }

        return await response.json();
    } catch (error) {
        console.error("Error starting plan:", error.message);
        throw error; // Re-throw to allow handling at a higher level
    }
};

export const addProcedureToPlan = async (plans) => {
    const url = `${api_url}/Plan/AddProcedureToPlan`;
    const response = await fetch(url, {
        method: "POST",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify(plans),
    });

    if (!response.ok) throw new Error("Failed to create plan");

    return true;
};

export const assignUserToProcedure = async (plans) => {
    const url = `${api_url}/UserProcedure/AddUserProcedureToPlan`;
    const response = await fetch(url, {
        method: "POST",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify(plans),
    });

    if (!response.ok) {
        throw new Error("Failed to assign user to procedure");
    }

    return true;
};

export const getProcedures = async () => {
    const url = `${api_url}/Procedures`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get procedures");

    return await response.json();
};

export const getPlanProcedures = async (planId) => {
    const url = `${api_url}/PlanProcedure?$filter=planId eq ${planId}&$expand=procedure,UserProcedures`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get plan procedures");

    const data = await response.json();  // Read the body once
    console.log(data);  // Log the data to inspect its structure

    return data;  // Return the data
};

export const getUsers = async () => {
    const url = `${api_url}/Users`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get users");

    return await response.json();
};