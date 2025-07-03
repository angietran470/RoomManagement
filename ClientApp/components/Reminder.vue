<template>
    <UModal
        title="Reminder"
    >
        <template #body>
            <ol class="list-inside list-decimal">
                <li v-for="event in events" :key="event">
                    Event: {{ event.name }}
                    Start: {{ new Date(event.startAt).toLocaleString() }} - End: {{ new Date(event.endAt).toLocaleTimeString() }}
                    Room: {{ event.roomName }}
                </li>
            </ol>
        </template>
    </UModal>
</template>

<script setup land="ts">
const events =ref()

let currentDate= new Date()

$fetch(`/api/v1/Event/Events?start=${currentDate.toLocaleDateString()}&end=${currentDate.toLocaleString()}`,{
    server: false,
    headers:{
        authorization:`Bearer ${localStorage.getItem('token')}`
    },
    onResponse({response}){
        events.value=response._data
    }
})

</script>