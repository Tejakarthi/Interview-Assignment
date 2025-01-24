import React from "react";
import ReactSelect from "react-select";

const PlanProcedureItem = ({ procedure, users, selectedUsers, onUserAssign }) => {
  const handleUserAssignment = (selectedOptions) => {
    onUserAssign(procedure.procedureId, selectedOptions);
  };

  return (
    <div className="py-2">
      <div>{procedure.procedureTitle}</div>
      <ReactSelect
        className="mt-2"
        placeholder="Select User to Assign"
        isMulti
        options={users}
        value={selectedUsers}
        onChange={handleUserAssignment}
      />
    </div>
  );
};

export default PlanProcedureItem;