from fastapi import FastAPI, Response
from img_toolkit import operations, read_img
import cv2 as cv

app = FastAPI()

@app.get('/image')
def apply(img_path, operation, value, img_type = 'color'):
    img, file_extension = read_img(img_path, img_type)

    if(int(value) == 0):
        _, encoded = cv.imencode(file_extension, img) 
        file_extension  = file_extension.replace('.', '')
        return Response(content=encoded.tobytes(), media_type=f'image/{file_extension}')

    result = operations[operation.lower()](img, int(value))
    _, encoded = cv.imencode(file_extension, result)
    file_extension  = file_extension.replace('.', '')
    return Response(content=encoded.tobytes(), media_type=f'image/{file_extension}')

@app.get('/grayscale')
def apply(img_path):
    img, file_extension = read_img(img_path, 'gray')
    _, encoded = cv.imencode(file_extension, img)
    file_extension  = file_extension.replace('.', '')
    return Response(content=encoded.tobytes(), media_type=f'image/{file_extension}')