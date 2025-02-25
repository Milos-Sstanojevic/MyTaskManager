import React, { useState, useEffect } from "react";
import { Calendar, dateFnsLocalizer } from "react-big-calendar";
import "react-big-calendar/lib/css/react-big-calendar.css";
import { format, parse, startOfWeek, getDay } from "date-fns";
import enUS from "date-fns/locale/en-US";

const locales = {
  "en-US": enUS,
};
const localizer = dateFnsLocalizer({
  format,
  parse,
  startOfWeek,
  getDay,
  locales,
});

const CalendarTab = ({ tasks }) => {
  const [events, setEvents] = useState([]);

  useEffect(() => {
    const transformedEvents = tasks.map((task, index) => ({
      id: task.id,
      title: task.taskName,
      start: new Date(task.startDate),
      end: new Date(task.endDate),
      color: getRandomColor(index),
    }));
    setEvents(transformedEvents);
  }, [tasks]);

  const getRandomColor = (index) => {
    const colors = ["#FF5733", "#33FF57", "#3357FF", "#F4D03F", "#9B59B6"];
    return colors[index % colors.length];
  };

  const eventStyleGetter = (event) => {
    return {
      style: {
        backgroundColor: event.color,
        borderRadius: "5px",
        opacity: 0.8,
        color: "white",
        border: "none",
        display: "block",
      },
    };
  };

  return (
    <div style={{ height: "80vh", width: "100%" }}>
      <Calendar
        localizer={localizer}
        events={events}
        startAccessor="start"
        endAccessor="end"
        style={{ width: "100%", height: "100%" }}
        eventPropGetter={eventStyleGetter}
      />
    </div>
  );
};

export default CalendarTab;
