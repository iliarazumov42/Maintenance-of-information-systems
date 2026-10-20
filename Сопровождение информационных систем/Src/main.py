from fastapi import FastAPI, UploadFile, File
from pydantic import BaseModel
from PIL import Image
import sqlite3
import os
import shutil
import modification_module
from datetime import datetime

app = FastAPI()

connection = sqlite3.connect("database.db", check_same_thread=False)
cursor = connection.cursor()

cursor.execute("""
CREATE TABLE IF NOT EXISTS images (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT,
    size INTEGER,
    width INTEGER,
    height INTEGER,
    type TEXT,
    date_added TEXT,
    path TEXT
)
""")

connection.commit()

# Модель для изменения размера
class ResizeRequest(BaseModel):
    path: str
    width: int
    height: int

# Модель для поворота
class RotateRequest(BaseModel):
    path: str
    angle: int

# 1. Добавление изображения
@app.post("/api/image/add")
async def add_image(file: UploadFile = File(...)):

    file_path = f"images/{file.filename}"

    with open(file_path, "wb") as buffer:
        shutil.copyfileobj(file.file, buffer)

    image = Image.open(file_path)

    width, height = image.size

    file_size = os.path.getsize(file_path)

    file_type = file.filename.split(".")[-1]

    date_added = datetime.now().strftime("%Y-%m-%d %H:%M:%S")

    cursor.execute("""
    INSERT INTO images
    (name, size, width, height, type, date_added, path)
    VALUES (?, ?, ?, ?, ?, ?, ?)
    """, (
        file.filename,
        file_size,
        width,
        height,
        file_type,
        date_added,
        file_path
    ))

    connection.commit()

    return {
        "message": "Изображение добавлено",
        "path": file_path
    }

# 2. Изменение размера
@app.put("/api/image/change/size")
def resize_image(data: ResizeRequest):

    result = modification_module.resize_picture(
        data.path,
        data.width,
        data.height
    )

    return {
        "message": "Размер изменен",
        "saved_path": result
    }


# 3. Поворот изображения
@app.put("/api/image/change/rotate")
def rotate_image(data: RotateRequest):

    result = modification_module.rotate_picture(
        data.path,
        data.angle
    )

    return {
        "message": "Изображение повернуто",
        "saved_path": result
    }


# 4. Получить все изображения
@app.get("/api/image")
def get_images():

    cursor.execute("SELECT * FROM images")

    images = cursor.fetchall()

    return images
