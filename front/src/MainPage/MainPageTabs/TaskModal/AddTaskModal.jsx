import React, { useState } from "react";
import Calendar from 'react-calendar';
import './AddTaskModal.css';
import 'react-calendar/dist/Calendar.css';

const linkForAddFetch = "http://localhost:5164/Task/CreateTask";

const AddTaskModal = ({ onClose, onAdd }) => {
    const [taskName, setTaskName] = useState("");
    const [description, setDescription] = useState("");
    const [stateOfTask, setStateOfTask] = useState(1);
    const [dates, setDates] = useState([new Date(), new Date()]);

    const handleOverlayClick = (e) => {
        if (e.target.classList.contains("modal-overlay")) {
            onClose();
        }
    };

    const handleSave = () => {
        const newTask = {
            taskName,
            description,
            stateOfTask,
            dateTaskStarted: dates[0],
            dateTaskShouldEnd: dates[1]
        };
       
        handleCreateNewTask(newTask);
    };

    const handleCreateNewTask = async (newTask) => {
        try {
            const response = await fetch(linkForAddFetch, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                },
                body: JSON.stringify({
                    taskName: newTask.taskName,
                    description: newTask.description,
                    stateOfTask: newTask.stateOfTask,
                    dateTaskStarted: newTask.dateTaskStarted,
                    dateTaskShouldEnd: newTask.dateTaskShouldEnd
                })
            });
            if (response.ok) {
                const taskFromApi = await response.json();
                alert("Successfully created new task!");
                onAdd(taskFromApi);
            }
            else {
                const errorData = await response.text();
                alert(`Error with creating new task: ${errorData || 'Unknown error'}`);
            }
        }
        catch (error) {
            console.error("Error with creating new task: ", error);
        }
    }

    return (
        <div className="modal-overlay" onClick={handleOverlayClick}>
            <div className="modal-content">
                <h2>Add New Task</h2>

                <label htmlFor="task-name">Task Name</label>
                <input 
                    type="text" 
                    id="task-name" 
                    value={taskName} 
                    onChange={(e) => setTaskName(e.target.value)} 
                    placeholder="Enter task name"
                />

                <label htmlFor="task-description">Description</label>
                <textarea 
                    id="task-description" 
                    value={description} 
                    onChange={(e) => setDescription(e.target.value)} 
                    placeholder="Enter task description"
                />

                <label htmlFor="task-state">State of Task</label>
                <select 
                    id="task-state" 
                    value={stateOfTask} 
                    onChange={(e) => setStateOfTask(Number(e.target.value))}
                >
                    <option value={1}>Urgent</option>
                    <option value={2}>Important</option>
                    <option value={3}>Next Day</option>
                    <option value={4}>Done</option>
                </select>

                <label>Task Dates</label>
                <Calendar 
                    onChange={setDates} 
                    value={dates} 
                    selectRange={true} 
                />

                <button className="save-task-btn" onClick={handleSave}>Save New Task</button>
            </div>
        </div>
    );
};

export default AddTaskModal;
