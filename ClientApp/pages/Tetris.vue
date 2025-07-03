<template>
    <div class="flex flex-col items-center p-4">
        <div class="bg-cyan-500"/>
        <div class="bg-blue-500"/>
        <div class="bg-green-500"/>
        <div class="bg-orange-500"/>
        <div class="bg-yellow-500"/>
        <div class="bg-purple-500"/>
        <div class="bg-red-500"/>

        <div class="grid grid-cols-10 gap-0.5 bg-gray-800">
            <div v-for="(row, y) in board" :key="y" class="contents">
                <div v-for="(cell, x) in row" :key="x" class="w-5 h-5 border border-gray-500" :class="cellClass(cell)">
                </div>
            </div>
        </div>
        Score: {{ score }}
    </div>
</template>

<script setup>
    const ROWS=20
    const COLS= 10
    const COLORS = ['cyan','blue', 'orange','yellow','green','red','purple']
    const SHAPES = [
        [
            [1, 1, 1, 1]
        ], // I
        [
            [2, 0, 0], 
            [2, 2, 2]
        ], // J
        [
            [0, 0, 3], 
            [3, 3, 3]
        ], // L
        [
            [4, 4], 
            [4, 4]
        ], // O
        [
            [0, 5, 5], 
            [5, 5, 0]
        ], // S
        [
            [0, 6, 0], 
            [6, 6, 6]
        ], // T
        [
            [7, 7, 0], 
            [0, 7, 7]
        ] // Z
    ]
    const randomPiece = () => {
        const idx = Math.floor(Math.random()*SHAPES.length)
        return{
            idx,
            shape: SHAPES[idx],
            x: Math.floor(COLS / 2)-1,
            y: 0
        }
    }
    let board = reactive(Array.from({length: ROWS}, () => Array(COLS).fill(0)))
    const current = reactive(randomPiece())
    const score= ref()

    const cellClass = (value) => {
        return value !=0 ? `bg-${COLORS[value-1]}-500` : 'bg-gray-800'
    }

    const draw = () => {
        current.shape.forEach((row, dy) => {
            row.forEach((val,dx) =>{
                if(val)board[ current.y + dy ][ current.x + dx ] = current.idx + 1
            })
        })
    }

    const unDraw = () => {
        current.shape.forEach((row, dy) => {
            row.forEach((val,dx)=>{
                 if (val) board [current.y+dy][current.x+dx] = 0
            })
        })
    }

    const collide = (xOffset=0, yOffset=0, shape = current.shape) =>{
        return shape.some((row,dy)=>
            row.some((val,dx)=>{
                if(!val) return false
                const x= current.x +dx + xOffset
                const y= current.y +dy + yOffset
                return (y>=ROWS || x>=COLS || x<0 || y<0 || board [y][x])
            })
        )
    }

    const rotate =(shape)=>{
        return shape[0].map(
            (_, idx)=>shape.map(row=>row[idx]).reverse()
        )
    }

    const freeze = () => {
        for(let y=ROWS -1; y>=0; y--){
            if(board[y].every(cell=>cell)){
                board.splice(y,1)
                board.unshift(Array(COLS).fill(0))
                score.value+= 10
                y++
            }
        }
        Object.assign(current,randomPiece())
        if(collide()){
            draw()
            alert ('You lost!')
            score.value=0
            board = reactive(Array.from({length: ROWS}, () => Array(COLS).fill(0)))
        } 
    }

    const drop = () => {
        unDraw()

        if(!collide(0,1)){
            current.y += 1 
        }
        else{
            draw() 
            freeze()
            return
        }

        draw()
    }

    const move = (dir) => {
        unDraw()
        if(!collide(dir,0)){
            current.x+=dir
        }
        draw()
    }

    const rotatePiece= () =>{
        unDraw()
        const nextPiece= rotate(current.shape)
        if(!collide(0,0, nextPiece)) current.shape=nextPiece
        draw()
    }

    
    const handelKey=(event)=>{
        if(event.key == 'ArrowUp') rotatePiece()
        if(event.key =='ArrowDown') drop()
        if(event.key =='ArrowRight') move(1)
        if(event.key =='ArrowLeft') move(-1)
    }

    const startGame= () => {
        draw()
        setInterval(drop,700)
        window.addEventListener('keydown', handelKey)
    }

    onMounted(startGame)

</script>