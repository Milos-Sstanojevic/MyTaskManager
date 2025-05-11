import React, { useState, useEffect } from "react";
import { Calendar, dateFnsLocalizer, Views } from "react-big-calendar";
import "react-big-calendar/lib/css/react-big-calendar.css";
import { format, startOfWeek, getDay } from "date-fns";
import enUS from "date-fns/locale/en-US";
import { parse } from "date-fns";

// Localization setup
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
  const [date, setDate] = useState(new Date());
  const [view, setView] = useState(Views.MONTH);

  const sanitizeDate = (dateStr) => {
    if (!dateStr) return null;
    const clean = dateStr.slice(0, 23);
    return new Date(clean);
  };

  useEffect(() => {
    const transformedEvents = tasks.map((task, index) => ({
      id: task.id,
      title: task.taskName,
      start: sanitizeDate(task.dateTaskStarted),
      end: sanitizeDate(task.dateTaskShouldEnd),
      color: getRandomColor(index),
    }));

    setEvents(transformedEvents);
  }, [tasks]);

  const getRandomColor = (index) => {
    const colors = ["#FF5733", "#33FF57", "#3357FF", "#F4D03F", "#9B59B6"];
    return colors[index % colors.length];
  };

  // Style each event with its assigned color
  const eventStyleGetter = (event) => {
    return {
      style: {
        backgroundColor: event.color,
        borderRadius: "5px",
        opacity: 0.9,
        color: "white",
        border: "none",
        display: "block",
        padding: "2px",
      },
    };
  };

  // Custom rendering of event content (show full task name)
  const CustomEvent = ({ event }) => {
    return (
      <div style={{ backgroundColor: event.color, padding: "2px", borderRadius: "4px", color: "white" }}>
        {event.title}
      </div>
    );
  };

  return (
    <div style={{ height: "80vh", width: "100%" }}>
      <Calendar
        localizer={localizer}
        events={events}
        startAccessor="start"
        endAccessor="end"
        date={date}
        view={view}
        onNavigate={(newDate) => setDate(newDate)}
        onView={(newView) => setView(newView)}
        style={{ width: "100%", height: "100%" }}
        eventPropGetter={eventStyleGetter}
        components={{ event: CustomEvent }}
      />
    </div>
  );
};

export default CalendarTab;