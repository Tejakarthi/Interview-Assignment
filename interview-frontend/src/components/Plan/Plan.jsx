import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import {
  addProcedureToPlan,
  getPlanProcedures,
  getProcedures,
  getUsers,
  assignUserToProcedure,
} from "../../api/api";
import Layout from "../Layout/Layout";
import ProcedureItem from "./ProcedureItem/ProcedureItem";
import PlanProcedureItem from "./PlanProcedureItem/PlanProcedureItem";

const Plan = () => {
  let { id } = useParams();
  const [procedures, setProcedures] = useState([]);
  const [planProcedures, setPlanProcedures] = useState([]);
  const [users, setUsers] = useState([]);
  const [assignedUsers, setAssignedUsers] = useState({}); // Track assigned users per procedure

  useEffect(() => {
    (async () => {
      try {
        const procedures = await getProcedures();
        const planProcedures = await getPlanProcedures(id);
        const users = await getUsers();

        // Convert users to dropdown format: { label: "John Doe", value: 1 }
        const userOptions = users.map((u) => ({ label: u.name, value: u.userId }));

        // Initialize assigned users based on userProcedures for each procedure
        const initialAssignedUsers = planProcedures.reduce((acc, planProcedure) => {
          const assignedUsersForProcedure = planProcedure?.userProcedures
            ?.map((userProcedure) => {
              const user = userOptions.find((u) => u.value === userProcedure.userId);
              return user;
            })
            .filter(Boolean); // Remove null values
          acc[planProcedure.procedureId] = assignedUsersForProcedure;
          return acc;
        }, {});

        setUsers(userOptions);
        setProcedures(procedures);
        setPlanProcedures(planProcedures);
        setAssignedUsers(initialAssignedUsers);
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    })();
  }, [id]);

  const handleAddProcedureToPlan = async (procedure, selectedUsers) => {
    const hasProcedureInPlan = planProcedures.some(
      (p) => p.procedureId === procedure.procedureId
    );
    if (hasProcedureInPlan) return;

    try {
      await addProcedureToPlan({
        planId: id,
        procedureId: procedure.procedureId,
        type: "ProcedureAdd",
      });

      if (Array.isArray(selectedUsers) && selectedUsers.length > 0) {
        for (const user of selectedUsers) {
          await assignUserToProcedure({
            planId: id,
            procedureId: procedure.procedureId,
            userId: user.value,
            type: "ProcedureUserAdd",
          });
        }
      }

      setPlanProcedures((prevState) => [
        ...prevState,
        {
          planId: id,
          procedureId: procedure.procedureId,
          procedure: {
            procedureId: procedure.procedureId,
            procedureTitle: procedure.procedureTitle,
          },
        },
      ]);

      setAssignedUsers((prevState) => ({
        ...prevState,
        [procedure.procedureId]: selectedUsers,
      }));
    } catch (error) {
      console.error("Error adding procedure to plan:", error);
    }
  };

  const handleUserAssign = async (procedureId, selectedUsers) => {
    const previousAssignedUsers = assignedUsers[procedureId] || [];

    // Find users to add/remove
    const newUsers = selectedUsers.filter(
      (user) => !previousAssignedUsers.some((prevUser) => prevUser.value === user.value)
    );
    const usersToRemove = previousAssignedUsers.filter(
      (user) => !selectedUsers.some((selectedUser) => selectedUser.value === user.value)
    );

    const ClearAll = selectedUsers.length === 0 ? 1 : 0;

    try {
      // Add new users
      for (const user of newUsers) {
        await assignUserToProcedure({
          planId: id,
          procedureId,
          userId: user.value,
          type: "ProcedureUserAdd",
          ClearAll,
        });
      }

      // Remove unselected users
      for (const user of usersToRemove) {
        await assignUserToProcedure({
          planId: id,
          procedureId,
          userId: user.value,
          type: "ProcedureUserRemove",
          ClearAll,
        });
      }

      // Update state
      setAssignedUsers((prevState) => ({
        ...prevState,
        [procedureId]: selectedUsers,
      }));
    } catch (error) {
      console.error("Error updating user assignments:", error);
    }
  };

  return (
    <Layout>
      <div className="container pt-4">
        <div className="d-flex justify-content-center">
          <h2>OEC Interview Frontend</h2>
        </div>
        <div className="row mt-4">
          <div className="col">
            <div className="card shadow">
              <h5 className="card-header">Repair Plan</h5>
              <div className="card-body">
                <div className="row">
                  <div className="col">
                    <h4>Procedures</h4>
                    <div>
                      {procedures.map((p) => (
                        <ProcedureItem
                          key={p.procedureId}
                          procedure={p}
                          handleAddProcedureToPlan={handleAddProcedureToPlan}
                          planProcedures={planProcedures}
                        />
                      ))}
                    </div>
                  </div>
                  <div className="col">
                    <h4>Added to Plan</h4>
                    <div>
                      {planProcedures.map((p) => (
                        <PlanProcedureItem
                          key={p.procedure.procedureId}
                          procedure={p.procedure}
                          users={users}
                          selectedUsers={assignedUsers[p.procedure.procedureId] || []}
                          onUserAssign={handleUserAssign}
                        />
                      ))}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default Plan;
