import React, { useEffect, useState } from "react";
import "./Card.css"
import {FaTimes,FaCheck} from "react-icons/fa"
import EditTaskModal from "../TaskModal/EditTaskModal";

const fetchLinkTask = "http://localhost:5164/Task/"
const fetchLinkUserData="http://localhost:5164/Login/GetCurrentUserData"
const deleteTask = "DeleteTaskByName"
const completeTask = "MarkTaskAsFinished"

export const StateOfTask = {
    0: 'Default',
    1: 'Urgent',
    2: 'Important',
    3: 'Next day',
    4: 'Done'
};

const Card = ({ task, onDeleteTask, onCompleteTask, onUpdateTask }) => {
    const [taskData, setTaskData] = useState(task);
    const [isHovered, setIsHovered] = useState(false);
    const [currentUser, setCurrentUser] = useState();
    const [isModalOpen, setIsModalOpen] = useState(false);

    useEffect(() => {
        fetchData();
    },[])

    const fetchData = async () => {
         try {
            const response = await fetch(fetchLinkUserData, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                },
            });

            if (response.ok) {
                const data = await response.json();
                setCurrentUser(data);
            }
            else
                console.error('Error with deleting task: ', response.statusText);
        }
        catch (error)
        {
            console.error('An error occurred during deletion of task', error);
        }
    }

    const handleDeleteTask = async (task) => {
        try {
            const response = await fetch(fetchLinkTask + deleteTask + "/" + encodeURIComponent(task.taskName), {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                },
            });

            if (response.ok) {
                onDeleteTask(task);
            }
            else
                console.error('Error with deleting task: ', response.statusText);
        }
        catch (error)
        {
            console.error('An error occurred during deletion of task', error);
        }
    }

    const handleTaskComplete = async (task) => {
        try {
            const response = await fetch(fetchLinkTask + completeTask + "/" + encodeURIComponent(task.taskName), {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                },
            });

            if (response.ok) {
            const updatedTask = { ...task, stateOfTask: 4 };
            onCompleteTask(updatedTask);
            setTaskData(updatedTask);
            }
            else
                console.error('Error with deleting task: ', response.statusText);
        }
        catch (error)
        {
            console.error('An error occurred during deletion of task', error);
        }
    }

    const handleCardOpenClose = (value) => {
        setIsModalOpen(value);
    }

    const handleEditTask = (task) => {
        if (task === null)
            return;

        setTaskData(task);
        onUpdateTask(task);
    }

    return (
        <>
        <div className="task-card" onClick={()=>handleCardOpenClose(true)} onMouseEnter={()=>setIsHovered(true)} onMouseLeave={()=>setIsHovered(false)}>
            <p className="task-name">{taskData.taskName}</p>
            <div className="task-info">
                <p className="state-ofTask">Urgency: {StateOfTask[taskData.stateOfTask ]}</p>
                <p className="owner-ofTask">Owner: {taskData.ownerOfTask}</p>
            </div>

            {isHovered && currentUser?.userName && taskData.ownerOfTask===currentUser.userName && (
                <div className="task-actions">
                        <button className="delete-btn" onClick={(e) => { e.stopPropagation(); handleDeleteTask(taskData) }}><FaTimes /></button>
                        {StateOfTask[taskData.stateOfTask] !== 'Done' && (<button className="complete-btn" onClick={(e) => { e.stopPropagation(); handleTaskComplete(taskData) }}><FaCheck /></button>)}
                </div>
            )}
        </div>

            {isModalOpen && (<EditTaskModal task={taskData} user={ currentUser } onClose={()=>handleCardOpenClose(false)} onEdit={handleEditTask}/>)}
        </>
    )
}

export default Card;