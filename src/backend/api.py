from fastapi import FastAPI, Response
from img_toolkit import operations, read_img
import cv2 as cv

app = FastAPI()

@app.get('/image')
def apply(img_path, operation, value, img_type):
    oper = operations[operation]
    img = read_img(img_path, img_type)
    result = img#oper(img, int(value))
    _, encoded = cv.imencode('.jpg', result)
    return Response(content=encoded.tobytes(), media_type='image/jpg')