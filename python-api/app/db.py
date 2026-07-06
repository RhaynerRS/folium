from __future__ import annotations

import sqlite3
import threading
from pathlib import Path


class JobStore:
    """Persists job rows to a SQLite file so the job list survives API restarts."""

    def __init__(self, db_path: Path):
        db_path.parent.mkdir(parents=True, exist_ok=True)
        self._lock = threading.Lock()
        self._conn = sqlite3.connect(str(db_path), check_same_thread=False)
        self._conn.execute("PRAGMA journal_mode=WAL")
        self._conn.execute(
            """
            CREATE TABLE IF NOT EXISTS jobs (
                id TEXT PRIMARY KEY,
                source_filename TEXT NOT NULL,
                target_language TEXT NOT NULL,
                submit_kind TEXT NOT NULL,
                concurrency INTEGER NOT NULL,
                created_at TEXT NOT NULL,
                status TEXT NOT NULL,
                progress REAL NOT NULL,
                error TEXT,
                warning TEXT,
                source_path TEXT,
                target_path TEXT
            )
            """
        )
        self._conn.commit()

    def upsert(self, row: dict) -> None:
        with self._lock:
            self._conn.execute(
                """
                INSERT INTO jobs (
                    id, source_filename, target_language, submit_kind, concurrency,
                    created_at, status, progress, error, warning, source_path, target_path
                ) VALUES (
                    :id, :source_filename, :target_language, :submit_kind, :concurrency,
                    :created_at, :status, :progress, :error, :warning, :source_path, :target_path
                )
                ON CONFLICT(id) DO UPDATE SET
                    status = excluded.status,
                    progress = excluded.progress,
                    error = excluded.error,
                    warning = excluded.warning
                """,
                row,
            )
            self._conn.commit()

    def load_all(self) -> list[dict]:
        with self._lock:
            cursor = self._conn.execute("SELECT * FROM jobs ORDER BY created_at")
            columns = [description[0] for description in cursor.description]
            return [dict(zip(columns, row)) for row in cursor.fetchall()]
