<template>
    <div>
        <div class="text-4xl font-bold mb-4">
            Room Management - Day View
        
        </div>
        <div class="mb-4">
            <Button label="Calendar" icon="ic:outline-calendar-month" @click="onClickCalendar" />
        </div>
        <div class="text-2xl font-bold">
             {{ route.query.date }}
        </div>
        <DayPilotCalendar :config="config" ref="calendarRef" />
        <EventForm ref="eventFormRef" :eventInfo="event" 
        v-if="isEventFormShown" @update="loadEvents"/>
    </div>
</template>

<script setup lang="ts">
import { DayPilot, DayPilotCalendar } from '@daypilot/daypilot-lite-vue'
import { ref, onMounted } from 'vue'
import {EventForm} from '#components'
import * as signalR from '@microsoft/signalr'

const route = useRoute()
const event= ref()
const isEventFormShown= ref(false)
const eventFormRef=ref()

const toast = useToast()
const overlay = useOverlay()
const modal= overlay.create(EventForm)

const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hub/Events', signalR.HttpTransportType.LongPolling)
    .withAutomaticReconnect()
    .build()

connection.on('NewEvent',()=> loadEvents())
connection.on('UpdatedEvent',()=>loadEvents())
connection.on('DeletedEvent',()=> loadEvents())

connection
    .start()
    .catch((err)=> console.error("SignalR Connection Error:", err))

const scroll= () => {
    isEventFormShown.value=true
    setTimeout(() => {
    eventFormRef.value.$el.scrollIntoView({behavior: 'smooth',block:'start'})
    },200)
    
}

if(localStorage.getItem('token')==undefined||localStorage.getItem('token')==null){
    navigateTo('/Login')
}

const config = ref({
    viewType: 'Resources',
    locale: 'en-us',
    headerHeight: 30,
    cellHeight: 30,
    businessBeginsHour: 6,
    businessEndsHour: 21,
    timeRangeSelectedHandling: 'Enabled',
    onTimeRangeSelected: async (args) => {
        /*const modal = await DayPilot.Modal.prompt('Create a new event:', 'Event 1')
        const calendar = args.control
        calendar.clearSelection()
        if (modal.canceled) { return }
        calendar.events.add({
            start: args.start,
            end: args.end,
            id: DayPilot.guid(),
            text: modal.result,
            resource: args.resource
        })*/
        //console.log(args)
        
            event.value={}
            event.value.startAt=args.start
            event.value.endAt=args.end
            event.value.roomId=args.resource
            modal.open({eventInfo: event.value })
            args.control.clearSelection()
        
    },
    eventDeleteHandling: 'Update',
    onEventDeleted: (args) => {
        console.log('Event deleted: ' + args.e.text())
        $fetch(`/api/v1/Event?id=${args.e.id()}`,{
            server:false,
             headers:{
                Authorization:`Bearer ${localStorage.getItem('token')}`
            },
            method: 'DELETE',
            onResponse({response}){
                if(!response.ok){
                    toast.add({title:`Could not delete event ${args.e.text()}.`})
                }
                else{
                    toast.add({title: `Event ${args.e.text()} was deleted successfully.`})
                }
                loadEvents()
            }
        })
    },
    eventMoveHandling: 'Update',
    onEventMoved: (args) => {
        console.log('Event moved: ' + args.e.text())

        $fetch(`/api/v1/Event?id=${args.e.data.id}`,{
            server:false,
             headers:{
                 Authorization:`Bearer ${localStorage.getItem('token')}`
            },
            onResponse({response}){
                let originalEvent=response._data
                originalEvent.roomId=args.newResource
                $fetch(`/api/v1/Event`,{
                    server:false,
                     headers:{
                         Authorization:`Bearer ${localStorage.getItem('token')}`
                     },
                    method:'PUT',
                    body: originalEvent,
                    onResponse({response}){
                        if(!response.ok){
                            toast.add({title: `Error: ${response._data}`})
                        }
                        else{
                            toast.add({title: `Event #${originalEvent.id} was moved successfully!`})
                        }
                        loadEvents()
                    }
                })
            }


        })
    },
    eventResizeHandling: 'Update',
    onEventResized: (args) => {
        console.log('Event resized: ' + args.e.text())

        $fetch(`/api/v1/Event?id=${args.e.data.id}`,{
            server:false,
             headers:{
                Authorization:`Bearer ${localStorage.getItem('token')}`
            },
            onResponse({response}){
                let originalEvent=response._data
                originalEvent.startAt=args.newStart
                originalEvent.endAt=args.newEnd
                $fetch(`/api/v1/Event`,{
                    server:false,
                     headers:{
                         Authorization:`Bearer ${localStorage.getItem('token')}`
                    },
                    method:'PUT',
                    body: originalEvent,
                    onResponse({response}){
                        if(!response.ok){
                            toast.add({title: `Error: ${response._data}`})
                        }
                        else{
                            toast.add({title: `Event #${originalEvent.id} was resized successfully!`})
                        }
                    }
                })
            }
        })
    },
    eventClickHandling: 'ContextMenu',
    eventRightClickHandling: 'ContextMenu',
    contextMenu: new DayPilot.Menu({
        items: [
            {
                text: 'Edit', onClick: (args) => {
                    let id= args.source.id()
                    isEventFormShown.value=false
                    $fetch(`/api/v1/Event?id=${id}`,{
                        server:false,
                         headers:{
                            Authorization:`Bearer ${localStorage.getItem('token')}`
                        },
                        onResponse({response}){
                            event.value=response._data
                            modal.open({eventInfo:event.value})
                        }
                    })
                }
            },
            {
                text: 'Delete', onClick: (args) => {
                    const dp = args.source.calendar
                    dp.events.remove(args.source)
                    console.log('Deleted through menu')
                    $fetch(`/api/v1/Event?id=${args.source.id()}`,{
                     server:false,
                      headers:{
                        Authorization:`Bearer ${localStorage.getItem('token')}`
                    },
                    method: 'DELETE',
                    onResponse({response}){
                     if(!response.ok){
                    toast.add({title: `Could not delete event ${args.source.text()}.`})
                }
                loadEvents()
            }
        })
                }
            }
        ]
    }),
})

const calendarRef = ref(null)

const loadEvents = () => {
    let events = []
    let date = route.query.date
    isEventFormShown.value=false
    config.value.startDate = new Date(date)
    $fetch(`/api/v1/Event/Events?start=${date}&end=${date}`, {
        server: false,
         headers:{
            Authorization:`Bearer ${localStorage.getItem('token')}`
        },
        onResponse({ response }) {
            for (let event of response._data) {
                let eventStart = new Date(event.startAt)
                let eventEnd = new Date(event.endAt)
                events.push({
                    id: event.id,
                    start: eventStart.setHours(eventStart.getHours() - 7),
                    end: eventEnd.setHours(eventEnd.getHours() - 7),
                    text: event.name,
                    resource: event.roomId
                })
            }
            config.value.events = events
        }
    })
}

const loadResources = () => {
    $fetch('/api/v1/Room/Rooms', {
        server: false,
         headers:{
            Authorization:`Bearer ${localStorage.getItem('token')}`
        },
        onResponse({ response }) {
            config.value.columns = response._data
        }
    })
}

const onClickCalendar = () => navigateTo('/')

onMounted(() => {
    loadEvents()        // load events
    loadResources()     // load rooms (resources)
})
</script>
