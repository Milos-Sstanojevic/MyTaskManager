import React, { useState } from "react";
import './AddTaskCard.css';
import AddTaskModal from "../TaskModal/AddTaskModal";

const AddTaskCard = ({onAddTask}) => {

    const [isModalOpen, setIsModalOpen] = useState(false);

    const handleAddNewTask = (task) => {
        onAddTask(task);
    }

    const handleOpenModal = () => {
        setIsModalOpen(true);
    }

    const handleCloseModal = () => {
        setIsModalOpen(false);
    }

    return (
        <div>
            <div className="task-card add-new-task-card" onClick={handleOpenModal}>
                <div className="plus-sign">+</div>
                    <p className="task-name">Add New Task</p>
            </div>
            {isModalOpen && (
                <AddTaskModal onClose={handleCloseModal} onAdd={handleAddNewTask} />
            )}
        </div>
    );
}

export default AddTaskCard;