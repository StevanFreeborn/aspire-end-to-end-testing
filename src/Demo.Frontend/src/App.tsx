import { useEffect, useState, type SubmitEvent } from "react";
import "./App.css";

type Todo = {
  id: number;
  task: string;
  completed: boolean;
};

function App() {
  const [todos, setTodos] = useState<Todo[]>([]);
  const [newTask, setNewTask] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [showLoadingUI, setShowLoadingUI] = useState(false);

  useEffect(() => {
    const controller = new AbortController();
    const signal = controller.signal;

    const loadingTimer = setTimeout(() => {
      setShowLoadingUI(true);
    }, 250);

    async function fetchTodos() {
      try {
        const response = await fetch("/api/todos", { signal });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        setTodos(data);
        setIsLoading(false);

        clearTimeout(loadingTimer);
      } catch (error) {
        if (error instanceof Error && error.name === "AbortError") {
          console.log("Fetch successfully aborted");
        } else {
          console.error("Failed to fetch todos:", error);
          setIsLoading(false);
          clearTimeout(loadingTimer);
        }
      }
    }

    fetchTodos();

    return () => {
      controller.abort();
      clearTimeout(loadingTimer);
    };
  }, []);

  async function addTodo(e: SubmitEvent<HTMLFormElement>) {
    e.preventDefault();

    if (!newTask.trim()) {
      return;
    }

    try {
      const response = await fetch("/api/todos", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ task: newTask }),
      });

      const createdTodo = await response.json();

      setTodos([...todos, createdTodo]);
      setNewTask("");
    } catch (error) {
      console.error("Failed to add todo:", error);
    }
  }

  async function toggleComplete(
    id: number,
    currentTask: string,
    currentStatus: boolean,
  ) {
    try {
      const response = await fetch(`/api/todos/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ task: currentTask, completed: !currentStatus }),
      });

      if (response.ok) {
        setTodos(
          todos.map((todo) =>
            todo.id === id ? { ...todo, completed: !currentStatus } : todo,
          ),
        );
      }
    } catch (error) {
      console.error("Failed to update todo:", error);
    }
  }

  async function deleteTodo(id: number) {
    try {
      const response = await fetch(`/api/todos/${id}`, {
        method: "DELETE",
      });

      if (response.ok) {
        setTodos(todos.filter((todo) => todo.id !== id));
      }
    } catch (error) {
      console.error("Failed to delete todo:", error);
    }
  }

  return (
    <div className="App">
      <h1>Todo Manager</h1>

      <form onSubmit={addTodo} className="todo-form">
        <input
          type="text"
          value={newTask}
          onChange={(e) => setNewTask(e.target.value)}
          placeholder="What needs to be done?"
          className="todo-input"
        />
        <button type="submit" className="todo-submit">
          Add Task
        </button>
      </form>

      <ul className="todo-list">
        {todos.map((todo) => (
          <li key={todo.id} className="todo-item">
            <div className="todo-item-content">
              <input
                id={`todo-${todo.id}`}
                name={`todo-${todo.id}`}
                type="checkbox"
                checked={todo.completed}
                onChange={() =>
                  toggleComplete(todo.id, todo.task, todo.completed)
                }
                className="todo-checkbox"
              />
              <label htmlFor={`todo-${todo.id}`} className="sr-only">
                Mark {todo.task} as {todo.completed ? "incomplete" : "complete"}
              </label>
              <span
                className={todo.completed ? "todo-text completed" : "todo-text"}
              >
                {todo.task}
              </span>
            </div>
            <button
              onClick={() => deleteTodo(todo.id)}
              className="todo-delete-btn"
            >
              Delete
            </button>
          </li>
        ))}
      </ul>

      {isLoading && showLoadingUI && <p>Loading tasks...</p>}
      {!isLoading && todos.length === 0 && <p>No tasks yet. Add one above!</p>}
    </div>
  );
}

export default App;
