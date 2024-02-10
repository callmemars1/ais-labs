from flask import Flask, request, redirect, url_for, render_template
import os

app = Flask(__name__)
data = []

@app.route('/')
def home():
    return render_template('index.html', data=data)

@app.route('/post', methods=['POST'])
def post_data():
    data.append(request.form['data'])
    return redirect(url_for('home'))

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=os.getenv('PORT', 5000))
