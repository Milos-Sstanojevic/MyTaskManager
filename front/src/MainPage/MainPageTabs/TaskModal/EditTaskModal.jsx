import React, { useEffect, useState } from "react";
import Calendar from 'react-calendar';
import './EditTaskModal.css';
import 'react-calendar/dist/Calendar.css';
import { StateOfTask } from "../Cards/Card";

const linkForEditFetch = "http://localhost:5164/Task/UpdateTaskInfo";
const linkForMembersOfTask = "http://localhost:5164/Task/GetAllMembersOfTask";
const linkForAddingMembers = "http://localhost:5164/Task/AddMembersToTheTaskByMail";
const linkForRemovingMember = "http://localhost:5164/Task/RemoveMemberFromTask";

const EditTaskModal = ({ task, user, onClose, onEdit }) => {
    const [taskData, setTaskData] = useState(task);
    const [emailsInput, setEmailsInput] = useState("");
    const [membersOfTask, setMembersOfTask] = useState([]);
    const [selectedMember, setSelectedMember] = useState(null);
    const [isEditing, setIsEditing] = useState(false);
    const [taskName, setTaskName] = useState(task.taskName);
    const [description, setDescription] = useState(task.description || "");
    const [stateOfTask, setStateOfTask] = useState(task.stateOfTask);
    const [dates, setDates] = useState([new Date(task.dateTaskStarted), new Date(task.dateTaskShouldEnd)]);

    const handleOverlayClick = (e) => {
        if (e.target.classList.contains("modal-overlay")) {
            onClose();
        }
    };

    useEffect(() => {
        getMembersOfTask();
    }, [])
    
    const getMembersOfTask = async () => {
        try {
            const response = await fetch(linkForMembersOfTask + "/" + taskName, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                },
            });
            if (response.ok) {
                const members = await response.json();
                const emails = members.map(m => m.member.email);
                console.log(emails);
                setMembersOfTask(emails);
            }
            else {
                console.log(await response.text());
            }
        }
        catch (error) {
            console.error("Error with getting members of this task: ", error);
        }
    }

    const handleSave = () => {
        const oldTaskName = taskData.taskName;
        const updatedTask = {
            ...taskData,
            taskName,
            description,
            stateOfTask,
            dateTaskStarted: dates[0],
            dateTaskShouldEnd: dates[1]
        };

        handleEdit(oldTaskName, updatedTask);
        setIsEditing(false);
    };

    const handleEdit = async (oldTaskName, updatedTask) => {
        try {
            const response = await fetch(linkForEditFetch + "/" + encodeURIComponent(oldTaskName), {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                },
                body: JSON.stringify({
                    taskName: updatedTask.taskName,
                    description: updatedTask.description,
                    stateOfTask: updatedTask.stateOfTask,
                    dateTaskStarted: updatedTask.dateTaskStarted,
                    dateTaskShouldEnd: updatedTask.dateTaskShouldEnd
                })
            });

            if (response.ok) {
                alert("Successfully updated task info");
                onEdit(updatedTask);
                setTaskData(updatedTask);
            }
            else {
                const errorData = await response.text();
                alert(`Error with editing task info: ${errorData || 'Unknown error'}`);
            }
        }
        catch (error) {
            console.error('Error with editing task data: ', error);
        }
    }

    const handleRemoveEmail = () => {
        if (!selectedMember) return;

        removeMember();
    }

    const removeMember = async () => {
        try {
            const response = await fetch(linkForRemovingMember + "/" + encodeURIComponent(selectedMember) + "/" + encodeURIComponent(taskName), {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                },
            });
            if (response.ok)
            {
                const responseText = await response.text();
                setMembersOfTask(membersOfTask.filter(email => email !== selectedMember));
                setSelectedMember(null);
                alert(responseText);
            }
            else 
                alert("Error with removing member");
        }
        catch (error) {
            console.error("Error with removing member: ", error);
        }
    }

    const handleAddEmails = () => {
        const newEmails = emailsInput
            .split(/[,\n]+/)
            .map(email => email.trim())
            .filter(email => email && !membersOfTask.includes(email));

        if (newEmails.length > 0)
            addMembersToTask(newEmails);
        
    }

    const addMembersToTask = async (newMembers) => {
        try {
            const response = await fetch(linkForAddingMembers + "/" + encodeURIComponent(taskName), {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                },
                body: JSON.stringify(newMembers),
            })
            if (response.ok) {
                setMembersOfTask([...membersOfTask, ...newMembers]);
                setEmailsInput("");
                alert("Successfully added new members to the task!");
            }
            else {
                const responseData = await response.text();
                alert(responseData || 'Unknown error');
            }
        }
        catch (error) {
            console.error("Error with adding members to task: ", error);
        }
    }

    return (
        <div className="modal-overlay" onClick={handleOverlayClick}>
            <div className="modal-content">
                {isEditing ? (
                    <input 
                        type="text" 
                        className="task-name-input" 
                        value={taskName} 
                        onChange={(e) => setTaskName(e.target.value)}
                    />
                ) : (
                    <h2 className="task-name">{taskData.taskName}</h2>
                )}
                
                <p className="task-owner">Owner: {taskData.ownerOfTask}</p>
                
                {isEditing ? (
                    <textarea 
                        className="task-description-input" 
                        value={description} 
                        onChange={(e) => setDescription(e.target.value)}
                    />
                ) : (
                    <p className="task-description">{taskData.description || "No description provided."}</p>
                )}

                {isEditing ? (
                    <select className="task-state-dropdown" value={stateOfTask} onChange={(e) => setStateOfTask(Number(e.target.value))}>
                        <option value={1}>Urgent</option>
                        <option value={2}>Important</option>
                        <option value={3}>Next Day</option>
                        <option value={4}>Done</option>
                    </select>
                ) : (
                    <p className="task-state">State: {StateOfTask[taskData.stateOfTask]}</p>
                )}

                <div className="task-calendar">
                    {isEditing ? (
                        <Calendar
                            onChange={setDates}
                            value={dates}
                            selectRange={true}
                        />
                    ) : (
                        <Calendar
                            value={dates}
                            tileClassName={({ date, view }) => 
                                view === "month" && (date.toDateString() === dates[0].toDateString() ? "start-date" :
                                date.toDateString() === dates[1].toDateString() ? "end-date" : null)
                            }
                        />
                    )}
                </div>

                <div className="members-list">
                    <h3>Task Members</h3>
                    {membersOfTask.length > 0 ? (
                        membersOfTask.map((email, index) => (
                            <div 
                                key={index} 
                                className={`member-item ${selectedMember === email ? "selected" : ""}`}
                                onClick={() => setSelectedMember(email)}
                            >
                                {email}
                            </div>
                        ))
                    ) : (
                        <p>No members added.</p>
                    )}
                </div>
                
                {isEditing && (
                    <button
                        className="remove-member-btn"
                        onClick={handleRemoveEmail}
                        disabled={!selectedMember}
                    >
                        Remove Selected Member
                    </button>
                )}

                {isEditing && (
                    <div className="email-input">
                        <textarea
                            value={emailsInput}
                            onChange={(e) => setEmailsInput(e.target.value)}
                            placeholder="Enter emails separated by commas or new lines"
                            rows="4"
                        />
                        <button onClick={handleAddEmails}>Add Members</button>
                    </div>
                )}

                {user.userName === taskData.ownerOfTask && (
                    isEditing ? (
                        <button className="save-task-btn" onClick={handleSave}>Save</button>
                    ) : (
                        <button className="edit-task-btn" onClick={() => setIsEditing(true)}>Edit Task</button>
                    )
                )}
            </div>
        </div>
    );
}

export default EditTaskModal;
