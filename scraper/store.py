"""
An append-only log of product snapshots, plus each site's scraping progress.
"""

import gzip
import json
import sqlite3

SCHEMA = """
CREATE TABLE IF NOT EXISTS snapshots (
    id INTEGER PRIMARY KEY,
    retailer TEXT NOT NULL,
    product_key TEXT NOT NULL,
    url TEXT NOT NULL,
    fetched_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data BLOB NOT NULL
);
CREATE INDEX IF NOT EXISTS snapshots_product ON snapshots (retailer, product_key, fetched_at);

CREATE TABLE IF NOT EXISTS progress (
    retailer TEXT PRIMARY KEY,
    next_page INTEGER NOT NULL
);
"""


def connect(path):
    conn = sqlite3.connect(path)
    conn.executescript(SCHEMA)
    return conn


def fresh_keys(conn, retailer, days):
    """Product keys with a snapshot from the last X days, so they don't need refetching."""
    rows = conn.execute(
        "SELECT DISTINCT product_key FROM snapshots "
        "WHERE retailer = ? AND fetched_at >= datetime('now', ?)",
        (retailer, f"-{days} days"),
    )
    return {key for (key,) in rows}


def save_snapshot(conn, retailer, product_key, url, data):
    with conn:
        conn.execute(
            "INSERT INTO snapshots (retailer, product_key, url, data) VALUES (?, ?, ?, ?)",
            (retailer, product_key, url, gzip.compress(json.dumps(data).encode())),
        )


def load_next_page(conn, retailer):
    row = conn.execute(
        "SELECT next_page FROM progress WHERE retailer = ?", (retailer,)
    ).fetchone()
    return row[0] if row else 1


def save_next_page(conn, retailer, page_number):
    with conn:
        conn.execute(
            "INSERT INTO progress (retailer, next_page) VALUES (?, ?) "
            "ON CONFLICT (retailer) DO UPDATE SET next_page = excluded.next_page",
            (retailer, page_number),
        )
