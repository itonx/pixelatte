from fastapi import FastAPI, Response
from img_toolkit import operations, read_img, get_img_with_salt_and_pepper
import cv2 as cv

app = FastAPI()

@app.get('/open')
def get_img(img_path, img_type = 'color'):
    img, file_extension, width, height = read_img(img_path, img_type)
    _, encoded = cv.imencode(file_extension, img)
    file_extension  = file_extension.replace('.', '')
    headers = {'width': str(width), 'height': str(height)}
    return Response(content=encoded.tobytes(), media_type=f'image/{file_extension}', headers=headers)

@app.get('/image')
def apply(img_path, operation, value, img_type = 'color'):
    img, file_extension, width, height = read_img(img_path, img_type)

    if(int(value) == 0):
        _, encoded = cv.imencode(file_extension, img) 
        file_extension  = file_extension.replace('.', '')
        return Response(content=encoded.tobytes(), media_type=f'image/{file_extension}')

    result = operations[operation.lower()](img, int(value))
    _, encoded = cv.imencode(file_extension, result)
    file_extension  = file_extension.replace('.', '')
    return Response(content=encoded.tobytes(), media_type=f'image/{file_extension}')

@app.get('/grayscale')
def grayscale(img_path):
    img, file_extension, width, height = read_img(img_path, 'gray')
    _, encoded = cv.imencode(file_extension, img)
    file_extension  = file_extension.replace('.', '')
    return Response(content=encoded.tobytes(), media_type=f'image/{file_extension}')

@app.get('/saltandpeppernoise')
def apply(img_path, noise_level, img_type = 'color'):
    img, file_extension, width, height = read_img(img_path, img_type)

    if(int(noise_level) == 0):
        _, encoded = cv.imencode(file_extension, img) 
        file_extension  = file_extension.replace('.', '')
        return Response(content=encoded.tobytes(), media_type=f'image/{file_extension}')

    result = get_img_with_salt_and_pepper(img, width, height, int(noise_level), img_type)
    _, encoded = cv.imencode(file_extension, result)
    file_extension  = file_extension.replace('.', '')
    return Response(content=encoded.tobytes(), media_type=f'image/{file_extension}')

@app.get('/convolution')
def convolution(img_path, img_type = 'color'):
    img, file_extension, width, height = read_img(img_path, img_type)
    # TODO: Add convolution implementation
    _, encoded = cv.imencode(file_extension, img) 
    file_extension  = file_extension.replace('.', '')
    return Response(content=encoded.tobytes(), media_type=f'image/{file_extension}')