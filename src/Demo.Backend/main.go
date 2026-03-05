package main

import (
	"database/sql"
	"encoding/json"
	"log"
	"net/http"
	"os"

	_ "github.com/lib/pq"
)

type Todo struct {
	ID        int    `json:"id"`
	Task      string `json:"task"`
	Completed bool   `json:"completed"`
}

func main() {
	connectionURIEnvVar := "DEMO_URI"
	connURI := os.Getenv(connectionURIEnvVar)

	if connURI == "" {
		log.Fatal("Environment variable " + connectionURIEnvVar + " is not set")
	}

	db, err := sql.Open("postgres", connURI+"?sslmode=disable")

	if err != nil {
		log.Fatal("Failed to open database:", err)
	}

	defer db.Close()

	if err := db.Ping(); err != nil {
		log.Fatal("Failed to connect to database:", err)
	}

	createTableSQL := `
	  CREATE TABLE IF NOT EXISTS todos (
		  id SERIAL PRIMARY KEY,
		  task VARCHAR(255) NOT NULL,
		  completed BOOLEAN DEFAULT FALSE
	  );
  `

	_, err = db.Exec(createTableSQL)

	if err != nil {
		log.Fatal("Failed to create todos table:", err)
	}

	mux := http.NewServeMux()

	mux.HandleFunc("GET /todos", func(w http.ResponseWriter, r *http.Request) {
		rows, err := db.Query("SELECT id, task, completed FROM todos ORDER BY id ASC")

		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}

		defer rows.Close()

		var todos []Todo

		for rows.Next() {
			var t Todo

			if err := rows.Scan(&t.ID, &t.Task, &t.Completed); err != nil {
				http.Error(w, err.Error(), http.StatusInternalServerError)
				return
			}

			todos = append(todos, t)
		}

		if todos == nil {
			todos = []Todo{}
		}

		w.Header().Set("Content-Type", "application/json")
		json.NewEncoder(w).Encode(todos)
	})

	mux.HandleFunc("POST /todos", func(w http.ResponseWriter, r *http.Request) {
		var t Todo

		if err := json.NewDecoder(r.Body).Decode(&t); err != nil {
			http.Error(w, "Invalid request body", http.StatusBadRequest)
			return
		}

		err := db.QueryRow(
			"INSERT INTO todos (task) VALUES ($1) RETURNING id, completed",
			t.Task,
		).Scan(&t.ID, &t.Completed)

		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}

		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusCreated)
		json.NewEncoder(w).Encode(t)
	})

	mux.HandleFunc("PUT /todos/{id}", func(w http.ResponseWriter, r *http.Request) {
		id := r.PathValue("id")

		var t Todo

		if err := json.NewDecoder(r.Body).Decode(&t); err != nil {
			http.Error(w, "Invalid request body", http.StatusBadRequest)
			return
		}

		res, err := db.Exec(
			"UPDATE todos SET task = $1, completed = $2 WHERE id = $3",
			t.Task, t.Completed, id,
		)

		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}

		rowsAffected, err := res.RowsAffected()

		if err != nil || rowsAffected == 0 {
			http.Error(w, "Todo not found", http.StatusNotFound)
			return
		}

		w.WriteHeader(http.StatusOK)
	})

	mux.HandleFunc("DELETE /todos/{id}", func(w http.ResponseWriter, r *http.Request) {
		id := r.PathValue("id")

		res, err := db.Exec("DELETE FROM todos WHERE id = $1", id)

		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}

		rowsAffected, err := res.RowsAffected()

		if err != nil || rowsAffected == 0 {
			http.Error(w, "Todo not found", http.StatusNotFound)
			return
		}

		w.WriteHeader(http.StatusNoContent)
	})

	port := os.Getenv("PORT")

	if port == "" {
		port = "8080"
	}

	log.Println("Server starting on port " + port + "...")

	log.Fatal(http.ListenAndServe(":"+port, mux))
}
