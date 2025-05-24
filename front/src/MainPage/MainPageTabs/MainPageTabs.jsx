import React, { useEffect, useState } from "react";
import "./MainPageTabs.css"
import Card, { StateOfTask } from "./Cards/Card";
import AddTaskCard from "./Cards/AddTaskCard";
import CalendarTab from "./CalendarTab/CalendarTab";

const fetchLink = "http://localhost:5164/Task/"

const MainPageTabs = () => {
    const [toggleState, setToggleState] = useState("Personal");
    const [linkToFetch, setLinkToFetch] = useState("GetAllPersonalTasksOfUser");
    const [fetchedTasks, setFetchedTasks] = useState([]);
    const [filter, setFilter] = useState("All");

    const toggleTab = (index) => {
        setToggleState(index)
        if (index === "Personal")
            setLinkToFetch("GetAllPersonalTasksOfUser");
        else if (index === "Team")
            setLinkToFetch("GetAllTeamTasksOfUser")
        else if (index === "MemberedTasks")
            setLinkToFetch("GetAllTasksUserIsMemberOf");
    }

    useEffect(() => {
        fetchData();
    }, [linkToFetch])


    const fetchData = async () => {
        try {
            const response = await fetch(fetchLink + linkToFetch, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                },
            });

            if (response.ok) {
                const data = await response.json();
                const processedData = data.map(task => ({
                    ...task,
                    ownerOfTask: task.ownerOfTask ? task.ownerOfTask.userName : 'Unknown',
                }));

                setFetchedTasks(processedData);
            }
            else {
                console.error('Error fetching data: ', response.statusText);
            }
        }
        catch (error) {
            console.error('An error occurred during data fetching: ', error);
        }
    }

    const handleOnDeleteTask = (deletedTask) => {
        setFetchedTasks((prevTasks) => prevTasks.filter(task => task.id !== deletedTask.id));
    }

    const handleOnCompleteTask = (completedTask) => {
        setFetchedTasks((prevTasks) => prevTasks.map((task) => task.id === completedTask.id ? completedTask : task));
    }

    const handleUpdateTask = (updatedTask) => {
        setFetchedTasks((prevTasks) => prevTasks.map(task => (task.id === updatedTask.id ? updatedTask : task)));
    }

    const handleAddNewTask = (task) => {
        const newTask = {
            ...task,
            ownerOfTask: task.ownerOfTask ? task.ownerOfTask.userName : 'Unknown',
        };
        setFetchedTasks((prevTasks) => [...prevTasks, newTask]);
    }

    const filteredTasks = filter === "All" ? fetchedTasks : fetchedTasks.filter(task => task.stateOfTask === filter);


    return (
        <div className="container-tabs">
            <div className="bloc-tabs">
                <div className={toggleState === "Personal" ? "tabs active-tabs" : "tabs"} onClick={() => toggleTab("Personal")}>Personal</div>
                <div className={toggleState === "Team" ? "tabs active-tabs" : "tabs"} onClick={() => toggleTab("Team")}>Team</div>
                <div className={toggleState === "MemberedTasks" ? "tabs active-tabs" : "tabs"} onClick={() => toggleTab("MemberedTasks")}>Membered Tasks</div>
                <div className={toggleState === "Calender" ? "tabs active-tabs" : "tabs"} onClick={() => toggleTab("Calender")}>Calender</div>
            </div>

            <div className="boards">
                {toggleState !== "Calender" &&
                    <div className="filter-dropdown">
                        <label>Filter: </label>
                        <select value={filter} onChange={(e) => setFilter(e.target.value)}>
                            <option value="All">All Tasks</option>
                            <option value={1}>Urgent</option>
                            <option value={2}>Important</option>
                            <option value={3}>Next Day</option>
                            <option value={4}>Done</option>
                        </select>
                    </div>
                }

                <div className={toggleState === "Personal" ? "board active-board" : "board"}>
                    {filteredTasks.length > 0 && filteredTasks.map((t) => (
                        <Card key={t.id} task={t} onDeleteTask={handleOnDeleteTask} onCompleteTask={handleOnCompleteTask} onUpdateTask={handleUpdateTask} />
                    ))
                    }
                    <AddTaskCard onAddTask={handleAddNewTask} />
                </div>

                <div className={toggleState === "Team" ? "board active-board" : "board"}>
                    {filteredTasks.length > 0 && filteredTasks.map((t) => (
                        <Card key={t.id} task={t} onDeleteTask={handleOnDeleteTask} onCompleteTask={handleOnCompleteTask} onUpdateTask={handleUpdateTask} />
                    ))
                    }
                    <AddTaskCard onAddTask={handleAddNewTask} />
                </div>

                <div className={toggleState === "MemberedTasks" ? "board active-board" : "board"}>
                    {filteredTasks.length > 0 && filteredTasks.map((t) => (
                        <Card key={t.id} task={t} onDeleteTask={handleOnDeleteTask} onCompleteTask={handleOnCompleteTask} />
                    ))
                    }
                </div>

                <div className={toggleState === "Calender" ? "board active-board" : "board"}>
                    <CalendarTab tasks={fetchedTasks} />
                </div>
            </div>
        </div>
    )
}

export default MainPageTabs;