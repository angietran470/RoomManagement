<template>
    <UModal
        title="Event Form"
    >
        <template #body>
        <FormKit
            type="form"
            :actions="false"
            @submit="onSubmit"
        >
            <FormKit
                label="Event Name"
                validation="required"
                validationVisibility="live"
                v-model="event.name"
            />
            <FormKit
                label= "Email"
                validation="required"
                validationVisibility="live"
                v-model="event.email"
                :disabled="true"
            />
            <FormKit
                label= "Phone"
                validation="required"
                validationVisibility="live"
                v-model="event.phone"
                :disabled="true"
            />     
          <FormKit
                type= "datetime-local"
                label="Start At"
                validation="required"
                validationVisibility="live"
                v-model="event.startAt"
            />
            <FormKit
                type= "datetime-local"
                label="End At"
                validation="required"
                validationVisibility="live"
                v-model="event.endAt"
            />

            <FormKit
                type= "select"
                label="Room"
                :options="rooms"
                validation="required"
                validationVisibility="live"
                v-if="rooms!=undefined"
                v-model="event.roomId"
            />

            <FormKit
                type="number"
                label="Attendees"
                validation="required|min:1"
                validationvisibility="live"
                v-model="event.attendees"
            />

             <FormKit
                type="number"
                label="Tabels"
                validation="required|min:0"
                validationvisibility="live"
                v-model="event.tables"
            />
            <FormKit
                type="number"
                label="Chairs"
                validation="required|min:0"
                validationvisibility="live"
                v-model="event.chairs"
            />
             <FormKit
                type="textarea"
                label="Comments"
            />
            <template v-if="props.eventInfo != undefined&& props.eventInfo.id != undefined">
                <Button label="Update Event" />
            </template>
            <template v-else>
                <Button label="New Event" />
            </template>
        
        </FormKit>
        </template>
    </UModal>

</template>

<script setup lang="ts">
const rooms = ref<any>()
const event= ref<any>({
    name:undefined,
    startAt:undefined,
    endAt:undefined,
    roomId:undefined,
    attendees: undefined,
    chairs:undefined,
    comments:undefined
})

const props= defineProps(['eventInfo'])
const emit = defineEmits(['update','close'])
const toast= useToast()

if(props.eventInfo != undefined ) {
    event.value=props.eventInfo
}
if(props.eventInfo!=undefined&& props.eventInfo.id==undefined){
    $fetch(`/api/v1/User?id=1`,{
        server: false,
         headers:{
            Authorization:`Bearer ${localStorage.getItem('token')}`
        },
        onResponse({response}){
            event.value.email=response._data.email
            event.value.phone= response._data.phone
            event.value.userName=response._data.userName
        }
    })
}

onMounted(()=>{
    $fetch('/api/v1/Room/Rooms',{
        server:false,
         headers:{
            Authorization:`Bearer ${localStorage.getItem('token')}`
        },
        onResponse({response}){
            let tempRooms=[{label:'Select a room', value: undefined}]
            for(let room of response._data){
            tempRooms.push({
                label:room.name,
                value:room.id
            })
            }
            rooms.value=tempRooms
        }
    })
})
const onSubmit = () => {
    if (props.eventInfo!=undefined && props.eventInfo.id!=undefined){
        $fetch('/api/v1/Event',{
            server:false,
             headers:{
                Authorization:`Bearer ${localStorage.getItem('token')}`
            },
            method:'PUT',
            body:event.value,
            onResponse({response}){
                if(!response.ok){
                    toast.add({title: `Error ${response._data}.`})
                }
                else{
                    emit('update')
                    toast.add({title: `Event #${event.value.id} was updated sucessfully!`})
                }

                emit(`close`)
            }
        })
    }
    else{
    $fetch('/api/v1/Event',{
        server:false,
         headers:{
            Authorization:`Bearer ${localStorage.getItem('token')}`
        },
        method:'POST',
        body: event.value,
        onResponse({response}){
            if(!response.ok){
                toast.add({title: `Error ${response._data}.`})
            }
            else{
                emit('update')
                toast.add({title: `Event #${response._data.id} was created sucessfully!`})
            }
            emit('close')
        }
    }
    )
}
}
</script>